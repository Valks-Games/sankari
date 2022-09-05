using ENet;

namespace Sankari.Netcode.Client;

using Event = ENet.Event;

public class GameClient : ENetClient
{
    public GameClient(Net networkManager, GodotCommands godotCmds) : base(networkManager)
    {
        _godotCmds = godotCmds;
    }

    protected override void Connecting()
    {
        Log("Client connecting...");
    }

    protected override void Receive(PacketReader reader)
    {
        _godotCmds.Enqueue(GodotOpcode.ENetPacket, new PacketInfo(reader, this));
    }

    protected override void Connect(ref Event netEvent)
    {
        Log("Client connected");
    }

    protected override void Leave(ref Event netEvent)
    {
        Log("Client left");
    }

    protected override void Stopped()
    {
        Log("Client stopped");
    }

    private void Log(object v) => Logger.Log($"[Client]: {v}", ConsoleColor.DarkGreen);
}
