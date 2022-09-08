namespace Sankari;

public class SPacketPlayerJoined : APacketServer 
{
    public string Username { get; set; }

    public override void Write(PacketWriter writer) 
    {
        writer.Write(Username);
    }

    public override void Read(PacketReader reader)
    {
        Username = reader.ReadString();
    }

    public override void Handle(GameClient client) 
    {
        GameManager.UIPlayerList.AddPlayer(Username);
    }
}