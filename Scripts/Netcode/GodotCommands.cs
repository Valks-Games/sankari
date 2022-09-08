using Sankari.Netcode;
using Sankari.Netcode.Client;

namespace Sankari;

public class GodotCommands
{
    private readonly ConcurrentQueue<GodotCmd> _godotCmdQueue = new();

    public void Enqueue(GodotOpcode opcode, object data = null) => _godotCmdQueue.Enqueue(new GodotCmd(opcode, data));

    public void Update()
    {
        if (_godotCmdQueue.TryDequeue(out var cmd))
        {
            switch (cmd.Opcode)
            {
                case GodotOpcode.ENetPacket:
                    var packetInfo = (PacketInfo)cmd.Data;
                    var packetReader = packetInfo.PacketReader;
                    var opcode = (ServerPacketOpcode)packetReader.ReadByte();

                    Logger.Log($"[Client]: Received {opcode}");

                    var handlePacket = ENetClient.HandlePacket[opcode];
                    handlePacket.Read(packetReader);

                    handlePacket.Handle(packetInfo.GameClient);

                    packetReader.Dispose();
                    break;
                case GodotOpcode.NetEvent:
                    GameManager.Notifications.Notify(GameManager.Linker, (Event)cmd.Data);
                    break;
                case GodotOpcode.SpawnPopupMessage:
                    var dataMessage = (GodotCmdPopupMessage)cmd.Data;
                    GameManager.Popups.SpawnMessage(dataMessage.Message, dataMessage.Title);
                    break;
                case GodotOpcode.SpawnPopupError:
                    var dataError = (GodotCmdPopupError)cmd.Data;
                    GameManager.Popups.SpawnError(dataError.Exception, dataError.Title);
                    break;
            }
        }
    }
}

public class GodotCmd
{
    public GodotOpcode Opcode { get; set; }
    public object Data { get; set; }

    public GodotCmd(GodotOpcode opcode, object data)
    {
        Opcode = opcode;
        Data = data;
    }
}

public enum GodotOpcode
{
    ENetPacket,
    SpawnPopupMessage,
    SpawnPopupError,
    NetEvent,
    ChangeScene,
    Disconnect
}

public class GodotCmdPopupMessage
{
    public string Title { get; set; }
    public string Message { get; set; }
}

public class GodotCmdPopupError
{
    public string Title { get; set; }
    public Exception Exception { get; set; }
}
