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
    private readonly ConcurrentQueue<ServerPacket> _outgoing = new();

    protected readonly Dictionary<uint, Peer> Peers = new();
    protected CancellationTokenSource CancellationTokenSource = new();
    protected bool queueRestart { get; set; }

    private long someoneConnected = 0;
    private long running = 0;
    private readonly Net networkManager;

    public ENetServer(Net networkManager)
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
    public void Send(ServerPacketOpcode opcode, params Peer[] peers) => Send(opcode, null, PacketFlags.Reliable, peers);
    public void Send(ServerPacketOpcode opcode, APacket data, params Peer[] peers) => Send(opcode, data, PacketFlags.Reliable, peers);
    public void Send(ServerPacketOpcode opcode, PacketFlags flags = PacketFlags.Reliable, params Peer[] peers) => Send(opcode, null, flags, peers);
    public void Send(ServerPacketOpcode opcode, APacket data, PacketFlags flags = PacketFlags.Reliable, params Peer[] peers) => _outgoing.Enqueue(new ServerPacket((byte)opcode, flags, data, peers));

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
            while (_outgoing.TryDequeue(out ServerPacket packet))
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
