using ENet;

namespace Sankari.Netcode.Client;

using Event = ENet.Event;

public abstract class ENetClient
{
    /// <summary>
    /// This property is not thread safe
    /// </summary>
    public static readonly Dictionary<ServerPacketOpcode, APacketServer> HandlePacket = ReflectionUtils.LoadInstances<ServerPacketOpcode, APacketServer>("SPacket");

    /// <summary>
    /// This property is thread safe
    /// </summary>
    public bool IsConnected => Interlocked.Read(ref connected) == 1;

    /// <summary>
    /// This property is thread safe
    /// </summary>
    public bool IsRunning => Interlocked.Read(ref running) == 1;

    protected ConcurrentQueue<ENetClientCmd> EnetCmds { get; } = new();
    private ConcurrentDictionary<int, ClientPacket> Outgoing { get; } = new();

    private int OutgoingId { get; set; }
    protected CancellationTokenSource CancellationTokenSource { get; set; } = new();
	
	// fields
    private long connected;
    private long running;

    /// <summary>
    /// This method is not thread safe
    /// </summary>
    public async void Start(string ip, ushort port) => await StartAsync(ip, port, CancellationTokenSource);

    /// <summary>
    /// This method is not thread safe
    /// </summary>
    public async Task StartAsync(string ip, ushort port, CancellationTokenSource cts)
    {
        try
        {
            if (IsRunning)
            {
                Logger.Log("Client is running already");
                return;
            }

            running = 1;
            CancellationTokenSource = cts;

            using var task = Task.Run(() => ENetThreadWorker(ip, port), CancellationTokenSource.Token);
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Client");
        }
    }

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public void Stop() => EnetCmds.Enqueue(new ENetClientCmd(ENetClientOpcode.Disconnect));

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public async Task StopAsync()
    {
        Stop();

        while (IsRunning)
            await Task.Delay(1);
    }

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public void Send(ClientPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        OutgoingId++;
        var success = Outgoing.TryAdd(OutgoingId, new ClientPacket((byte)opcode, flags, data));

        if (!success)
            Logger.LogWarning($"Failed to add {opcode} to Outgoing queue because of duplicate key");
    }

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public async Task SendAsync(ClientPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        Send(opcode, data, flags);

        while (Outgoing.ContainsKey(OutgoingId))
            await Task.Delay(1);
    }

    /// <summary>
    /// Thread safe way of executing client code from other threads.  
    /// Do not bring non-client vars over to the client thread. 
    /// Only work with variables that are in the clients thread. 
    /// </summary>
    public void ExecuteCode(Action<GameClient> action) => EnetCmds.Enqueue(new ENetClientCmd(ENetClientOpcode.ExecuteCode, action));

    protected virtual void Connecting() { }
    protected virtual void ClientCmds(Peer peer) { }
    protected virtual void Connect(ref Event netEvent) { }
    protected virtual void Disconnect(ref Event netEvent) { }
    protected virtual void Receive(PacketReader reader) { }
    protected virtual void Timeout(ref Event netEvent) { }
    protected virtual void Leave(ref Event netEvent) { }
    protected virtual void Sent(ClientPacketOpcode opcode) { }
    protected virtual void Stopped() { }

    private Task ENetThreadWorker(string ip, ushort port)
    {
        using var client = new Host();
        var address = new Address();
        address.SetHost(ip);
        address.Port = port;
        client.Create();

        Connecting();
        var peer = client.Connect(address);

        uint pingInterval = 1000; // Pings are used both to monitor the liveness of the connection and also to dynamically adjust the throttle during periods of low traffic so that the throttle has reasonable responsiveness during traffic spikes.
        uint timeout = 5000; // Will be ignored if maximum timeout is exceeded
        uint timeoutMinimum = 5000; // The timeout for server not sending the packet to the client sent from the server
        uint timeoutMaximum = 5000; // The timeout for server not receiving the packet sent from the client

        peer.PingInterval(pingInterval);
        peer.Timeout(timeout, timeoutMinimum, timeoutMaximum);

        while (!CancellationTokenSource.IsCancellationRequested)
        {
            var polled = false;

            // ENet Cmds from Godot Thread
            ClientCmds(peer);

            // Outgoing
            while (Outgoing.TryRemove(OutgoingId, out var clientPacket))
            {
                OutgoingId--;
                byte channelID = 0; // The channel all networking traffic will be going through
                var packet = default(Packet);
                packet.Create(clientPacket.Data, clientPacket.PacketFlags);
                peer.Send(channelID, ref packet);
                Sent((ClientPacketOpcode)clientPacket.Opcode);
            }

            while (!polled)
            {
                if (client.CheckEvents(out var netEvent) <= 0)
                {
                    if (client.Service(15, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                switch (netEvent.Type)
                {
                    case EventType.Connect:
                        connected = 1;
                        Connect(ref netEvent);
                        break;

                    case EventType.Receive:
                        // Receive
                        var packet = netEvent.Packet;
                        if (packet.Length > GamePacket.MaxSize)
                        {
                            Logger.LogWarning($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
                            packet.Dispose();
                            continue;
                        }

                        Receive(new PacketReader(packet));
                        break;

                    case EventType.Timeout:
                        connected = 0;
                        CancellationTokenSource.Cancel();
                        Timeout(ref netEvent);
                        Leave(ref netEvent);
                        break;

                    case EventType.Disconnect:
                        connected = 0;
                        CancellationTokenSource.Cancel();
                        Disconnect(ref netEvent);
                        Leave(ref netEvent);
                        break;
                }
            }

            client.Flush();
        }

        running = 0;

        Stopped();

        return Task.FromResult(1);
    }
}

public class PacketInfo
{
    public PacketReader PacketReader { get; set; }
    public GameClient GameClient { get; set; }

    public PacketInfo(PacketReader reader, GameClient client)
    {
        PacketReader = reader;
        GameClient = client;
    }
}

public class ENetClientCmd
{
    public ENetClientOpcode Opcode { get; set; }
    public object Data { get; set; }

    public ENetClientCmd(ENetClientOpcode opcode, object data = null)
    {
        Opcode = opcode;
        Data = data;
    }
}

public enum ENetClientOpcode
{
    Disconnect,
    ExecuteCode
}
