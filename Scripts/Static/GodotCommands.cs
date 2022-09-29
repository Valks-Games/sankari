namespace Sankari;

public static class GodotCommands
{
    private static ConcurrentQueue<GodotCmd> GodotCmdQueue { get; } = new();

    public static void Enqueue(GodotOpcode opcode, object data = null) => GodotCmdQueue.Enqueue(new GodotCmd(opcode, data));

    public static async Task Update()
    {
        if (GodotCmdQueue.TryDequeue(out var cmd))
        {
            switch (cmd.Opcode)
            {
                case GodotOpcode.ENetPacket:
                    var packetInfo = (PacketInfo)cmd.Data;
                    var packetReader = packetInfo.PacketReader;
                    var opcode = (ServerPacketOpcode)packetReader.ReadByte();

                    var logOpcode = true;

                    // TODO: Convert to Godot4
                    /*if (GameManager.Linker.IgnoreOpcodesFromServer != null)
                        foreach (var dontLogOpcode in GameManager.Linker.IgnoreOpcodesFromServer) 
                            if (opcode == dontLogOpcode) 
                            {
                                logOpcode = false;
                                break;
                            }*/

                    if (logOpcode)
                        Logger.Log($"[Client] Received: {opcode}");

                    // WARN: Not a thread safe way of getting HandlePacket
                    // Should not effect anything
                    var handlePacket = ENetClient.HandlePacket[opcode];
                    handlePacket.Read(packetReader);

                    await handlePacket.Handle();

                    packetReader.Dispose();
                    break;
                case GodotOpcode.NetEvent:
                    Notifications.Notify((Event)cmd.Data);
                    break;
                case GodotOpcode.SpawnPopupMessage:
                    var dataMessage = (GodotCmdPopupMessage)cmd.Data;
                    Popups.SpawnMessage(dataMessage.Message, dataMessage.Title);
                    break;
                case GodotOpcode.SpawnPopupError:
                    var dataError = (GodotCmdPopupError)cmd.Data;
                    Popups.SpawnError(dataError.Exception, dataError.Title);
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
    ChangeSceneToFile,
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
