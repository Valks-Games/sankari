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
        GodotCommands.Enqueue(GodotOpcode.ENetPacket, new PacketInfo(reader, this));
    }

    protected override void Connect(ref Event netEvent)
    {
        GodotCommands.Enqueue(GodotOpcode.NetEvent, Sankari.EventGeneric.OnGameClientConnected);
        Log("Client connected");
    }

    protected override void Leave(ref Event netEvent)
    {
        Log("Client left");
    }

    protected override void Stopped()
    {
        GodotCommands.Enqueue(GodotOpcode.NetEvent, Sankari.EventGeneric.OnGameClientStopped);
        Log("Client stopped");
    }

    public void Log(object v) => Logger.Log($"[Client] {v}", ConsoleColor.Cyan);
}
