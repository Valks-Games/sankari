using ENet;

namespace Sankari.Netcode.Server;

using Event = ENet.Event;

public abstract class ENetServer
{
    protected static readonly Dictionary<ClientPacketOpcode, APacketClient> HandlePacket = ReflectionUtils.LoadInstances<ClientPacketOpcode, APacketClient>("CPacket");

    // thread safe props
    public bool HasSomeoneConnected => Interlocked.Read(ref someoneConnected) == 1;
    public bool IsRunning => Interlocked.Read(ref running) == 1;
    public readonly ConcurrentQueue<ENetServerCmd> ENetCmds = new();
    private readonly ConcurrentQueue<ServerPacket> outgoing = new();

    protected readonly Dictionary<uint, Peer> Peers = new();
    protected CancellationTokenSource CancellationTokenSource = new();
    protected bool queueRestart { get; set; }

    private long someoneConnected = 0;
    private long running = 0;
    private readonly Net networkManager;

    protected ENetServer(Net networkManager)
    {
        this.networkManager = networkManager;
    }

    public async Task StartAsync(ushort port, int maxClients, CancellationTokenSource cts)
    {
        try
        {
            if (IsRunning)
            {
                Logger.Log("Server is running already");
                return;
            }

            running = 1;
            CancellationTokenSource = cts;

            using var task = Task.Run(() => ENetThreadWorker(port, maxClients), CancellationTokenSource.Token);
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Server");
        }
    }

    public void KickAll(DisconnectOpcode opcode)
    {
        Peers.Values.ForEach(peer => peer.DisconnectNow((uint)opcode));
        Peers.Clear();
    }

    public void Kick(uint id, DisconnectOpcode opcode)
    {
        Peers[id].DisconnectNow((uint)opcode);
        Peers.Remove(id);
    }

    public void Stop() => ENetCmds.Enqueue(new ENetServerCmd(ENetServerOpcode.Stop));
    public async Task StopAsync()
    {
        Stop();

        while (IsRunning)
            await Task.Delay(1);
    }
    public void Restart() => ENetCmds.Enqueue(new ENetServerCmd(ENetServerOpcode.Restart));
    
    /// <summary>
    /// Send an opcode to peer(s)
    /// </summary>
    public void Send(ServerPacketOpcode opcode, Peer peer, params Peer[] peers) =>
        outgoing.Enqueue(new ServerPacket((byte)opcode, PacketFlags.Reliable, null, JoinPeers(peer, peers)));

    /// <summary>
    /// Send an opcode to peer(s) with data
    /// </summary>
    public void Send(ServerPacketOpcode opcode, APacket data, Peer peer, params Peer[] peers) =>
        outgoing.Enqueue(new ServerPacket((byte)opcode, PacketFlags.Reliable, data, JoinPeers(peer, peers)));

    /// <summary>
    /// Send an opcode to peer(s) and specify how the packet is sent
    /// </summary>
    public void Send(ServerPacketOpcode opcode, PacketFlags flags, Peer peer, params Peer[] peers) => 
        outgoing.Enqueue(new ServerPacket((byte)opcode, flags, null, JoinPeers(peer, peers)));

    /// <summary>
    /// Send an opcode to peer(s) with data and specify how the packet is sent
    /// </summary>
    public void Send(ServerPacketOpcode opcode, APacket data, PacketFlags flags, Peer peer, params Peer[] peers) => 
        outgoing.Enqueue(new ServerPacket((byte)opcode, flags, data, JoinPeers(peer, peers)));

    // Same methods below but instead of the params being of type Peer, they are of type byte

    /// <summary>
    /// Send an opcode to peer(s)
    /// </summary>
    public void Send(ServerPacketOpcode opcode, byte peerId, params byte[] peerIds) =>
        outgoing.Enqueue(new ServerPacket((byte)opcode, PacketFlags.Reliable, null, JoinPeers(Peers[peerId], ConvertPeerIdsToPeers(peerIds))));

    /// <summary>
    /// Send an opcode to peer(s) with data
    /// </summary>
    public void Send(ServerPacketOpcode opcode, APacket data, byte peerId, params byte[] peerIds) =>
        outgoing.Enqueue(new ServerPacket((byte)opcode, PacketFlags.Reliable, data, JoinPeers(Peers[peerId], ConvertPeerIdsToPeers(peerIds))));

    /// <summary>
    /// Send an opcode to peer(s) and specify how the packet is sent
    /// </summary>
    public void Send(ServerPacketOpcode opcode, PacketFlags flags, byte peerId, params byte[] peerIds) =>
        outgoing.Enqueue(new ServerPacket((byte)opcode, flags, null, JoinPeers(Peers[peerId], ConvertPeerIdsToPeers(peerIds))));

    /// <summary>
    /// Send an opcode to peer(s) with data and specify how the packet is sent
    /// </summary>
    public void Send(ServerPacketOpcode opcode, APacket data, PacketFlags flags, byte peerId, params byte[] peerIds) => 
        outgoing.Enqueue(new ServerPacket((byte)opcode, flags, data, JoinPeers(Peers[peerId], ConvertPeerIdsToPeers(peerIds))));

    private Peer[] ConvertPeerIdsToPeers(byte[] peerIds) 
    {
        var peers = new Peer[peerIds.Length];
        
        for (int i = 0; i < peerIds.Length; i++)
            peers[i] = Peers[peerIds[i]];

        return peers;
    }   

    protected Peer[] GetOtherPeers(uint id)
    {
        var otherPeers = new Dictionary<uint, Peer>(Peers);
        otherPeers.Remove(id);
        return otherPeers.Values.ToArray();
    }

    protected virtual void Started(ushort port, int maxClients) { }
    protected virtual void Connect(ref Event netEvent) { }
    protected virtual void Received(Peer peer, PacketReader packetReader, ClientPacketOpcode opcode) { }
    protected virtual void Disconnect(ref Event netEvent) { }
    protected virtual void Timeout(ref Event netEvent) { }
    protected virtual void Leave(ref Event netEvent) { }
    protected virtual void Stopped() { }
    protected virtual void ServerCmds() { }

    private Task ENetThreadWorker(ushort port, int maxClients)
    {
        using var server = new Host();
        var address = new Address
        {
            Port = port
        };

        try
        {
            server.Create(address, maxClients);
        }
        catch (InvalidOperationException e)
        {
            var message = $"A server is running on port {port} already! {e.Message}";
            Logger.LogWarning(message);
            Cleanup();
            return Task.FromResult(1);
        }

        Started(port, maxClients);

        while (!CancellationTokenSource.IsCancellationRequested)
        {
            var polled = false;

            // ENet Cmds
            ServerCmds();

            // Outgoing
            while (outgoing.TryDequeue(out ServerPacket packet)) 
                packet.Peers.ForEach(peer => Send(packet, peer));

            while (!polled)
            {
                if (server.CheckEvents(out Event netEvent) <= 0)
                {
                    if (server.Service(15, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                var peer = netEvent.Peer;
                var eventType = netEvent.Type;

                switch (eventType)
                {
                    case EventType.Receive:
                        var packet = netEvent.Packet;
                        if (packet.Length > GamePacket.MaxSize)
                        {
                            Logger.LogWarning($"Tried to read packet from client of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
                            packet.Dispose();
                            continue;
                        }

                        var packetReader = new PacketReader(packet);
                        var opcode = (ClientPacketOpcode)packetReader.ReadByte();
                        Received(netEvent.Peer, packetReader, opcode);

                        packetReader.Dispose();
                        break;

                    case EventType.Connect:
                        someoneConnected = 1;
                        Peers[netEvent.Peer.ID] = netEvent.Peer;
                        Connect(ref netEvent);
                        break;

                    case EventType.Disconnect:
                        Peers.Remove(netEvent.Peer.ID);
                        Disconnect(ref netEvent);
                        Leave(ref netEvent);
                        break;

                    case EventType.Timeout:
                        Peers.Remove(netEvent.Peer.ID);
                        Timeout(ref netEvent);
                        Leave(ref netEvent);
                        break;
                }
            }
        }

        server.Flush();
        Cleanup();

        if (queueRestart)
        {
            queueRestart = false;
            networkManager.StartServer(port, maxClients, CancellationTokenSource);
        }

        return Task.FromResult(1);
    }

    private void Send(ServerPacket gamePacket, Peer peer)
    {
        var packet = default(Packet);
        packet.Create(gamePacket.Data, gamePacket.PacketFlags);
        byte channelID = 0;
        peer.Send(channelID, ref packet);
    }

    /// <summary>
    /// Joins peer and peers into one array. If peer is default(Peer) then peers is returned.
    /// </summary>
    private Peer[] JoinPeers(Peer peer, Peer[] peers) 
    {
        Peer[] thePeers = new Peer[0];

        if (peer.Equals(default(Peer)))
        {
            thePeers = peers;
        }
        else 
        {
            thePeers = new Peer[1 + peers.Length];
            thePeers[0] = peer;
            for (int i = 0; i < peers.Length; i++)
                thePeers[i + 1] = peers[i];
        }

        return thePeers;
    }

    private void Cleanup()
    {
        running = 0;
        Stopped();
    }
}

public class ENetServerCmd
{
    public ENetServerOpcode Opcode { get; set; }
    public object Data { get; set; }

    public ENetServerCmd(ENetServerOpcode opcode, object data = null)
    {
        Opcode = opcode;
        Data = data;
    }
}

public enum ENetServerOpcode
{
    Stop,
    Restart
}
