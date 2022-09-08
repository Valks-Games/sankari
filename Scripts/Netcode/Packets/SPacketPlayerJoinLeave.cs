namespace Sankari;

/// <summary>
/// Tell a client that this person is playing with us.
/// </summary>
public class SPacketPlayerJoinLeave : APacketServer 
{
    public string Username { get; set; }
    public bool Joining { get; set; }

    public override void Write(PacketWriter writer) 
    {
        writer.Write(Username);
        writer.Write(Joining);
    }

    public override void Read(PacketReader reader)
    {
        Username = reader.ReadString();
        Joining = reader.ReadBool();
    }

    public override void Handle(GameClient client) 
    {
        if (Joining)
            GameManager.UIPlayerList.AddPlayer(Username);
        else
            GameManager.UIPlayerList.RemovePlayer(Username);
    }
}