using ENet;

namespace Sankari.Netcode.Client;

using Event = ENet.Event;

public abstract class ENetClient
{
    public static readonly Dictionary<ServerPacketOpcode, APacketServer> HandlePacket = ReflectionUtils.LoadInstances<ServerPacketOpcode, APacketServer>("SPacket");

    // thread safe props
    public bool IsConnected => Interlocked.Read(ref connected) == 1;
    public bool IsRunning => Interlocked.Read(ref running) == 1;
    private readonly ConcurrentQueue<ENetClientCmd> enetCmds = new();
    private readonly ConcurrentDictionary<int, ClientPacket> outgoing = new();

    protected GodotCommands godotCmds;
    protected readonly Net networkManager;

    private long connected;
    private long running;
    private int outgoingId;
    private CancellationTokenSource cancellationTokenSource = new();

    public ENetClient(Net networkManager)
    {
        this.networkManager = networkManager;
    }

    public async void Start(string ip, ushort port) => await StartAsync(ip, port, cancellationTokenSource);

    public async Task StartAsync(string ip, ushort port, CancellationTokenSource cts)
    {
        try
        {
            if (IsRunning)
            {
                Logger.Log($"Client is running already");
                return;
            }

            running = 1;
            cancellationTokenSource = cts;

            using var task = Task.Run(() => ENetThreadWorker(ip, port), cancellationTokenSource.Token);
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Client");
        }
    }

    public void Stop() => enetCmds.Enqueue(new ENetClientCmd(ENetClientOpcode.Disconnect));

    public async Task StopAsync()
    {
        Stop();

        while (IsRunning)
            await Task.Delay(1);
    }

    public void Send(ClientPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        outgoingId++;
        var success = outgoing.TryAdd(outgoingId, new ClientPacket((byte)opcode, flags, data));

        if (!success)
            Logger.LogWarning($"Failed to add {opcode} to Outgoing queue because of duplicate key");
    }

    public async Task SendAsync(ClientPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        Send(opcode, data, flags);

        while (outgoing.ContainsKey(outgoingId))
            await Task.Delay(1);
    }

    protected virtual void Connecting() { }
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

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var polled = false;

            // ENet Cmds from Godot Thread
            while (enetCmds.TryDequeue(out ENetClientCmd cmd))
            {
                switch (cmd.Opcode)
                {
                    case ENetClientOpcode.Disconnect:
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            Logger.LogWarning("Client is in the middle of stopping");
                            break;
                        }

                        cancellationTokenSource.Cancel();
                        peer.Disconnect(0);
                        break;
                }
            }

            // Outgoing
            while (outgoing.TryRemove(outgoingId, out var clientPacket))
            {
                outgoingId--;
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
                        cancellationTokenSource.Cancel();
                        Timeout(ref netEvent);
                        Leave(ref netEvent);
                        break;

                    case EventType.Disconnect:
                        cancellationTokenSource.Cancel();
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
    Disconnect
}
