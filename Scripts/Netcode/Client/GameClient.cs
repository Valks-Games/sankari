namespace Sankari.Netcode.Client;

using Event = ENet.Event;

public class GameClient : ENetClient
{
    /// <summary>
    /// This property is not thread safe
    /// </summary>
    public bool TryingToConnect { get; set; }

    /// <summary>
    /// This property is not thread safe
    /// </summary>
    public byte PeerId { get; set; }

    public GameClient(GodotCommands godotCmds) : base()
    {
        GodotCmds = godotCmds;
    }

    protected override void Connecting()
    {
        Log("Client connecting...");
    }

    protected override void ClientCmds(ENet.Peer peer)
    {
        while (EnetCmds.TryDequeue(out ENetClientCmd cmd))
        {
            switch (cmd.Opcode)
            {
                case ENetClientOpcode.Disconnect:
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        Logger.LogWarning("Client is in the middle of stopping");
                        break;
                    }

                    CancellationTokenSource.Cancel();
                    peer.Disconnect(0);
                    break;

                case ENetClientOpcode.ExecuteCode:
                    var action = (Action<GameClient>)cmd.Data;
                    action(this);
                    break;
            }
        }
    }

    protected override void Receive(PacketReader reader)
    {
        GodotCmds.Enqueue(GodotOpcode.ENetPacket, new PacketInfo(reader, this));
    }

    protected override void Connect(ref Event netEvent)
    {
        GodotCmds.Enqueue(GodotOpcode.NetEvent, Sankari.Event.OnGameClientConnected);
        Log("Client connected");
    }

    protected override void Leave(ref Event netEvent)
    {
        Log("Client left");
    }

    protected override void Stopped()
    {
        GodotCmds.Enqueue(GodotOpcode.NetEvent, Sankari.Event.OnGameClientStopped);
        Log("Client stopped");
    }

    private void Log(object v) => Logger.Log($"[Client] {v}", ConsoleColor.DarkGreen);
}
