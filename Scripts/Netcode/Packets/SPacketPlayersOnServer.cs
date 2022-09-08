namespace Sankari;

/// <summary>
/// Tell a client every player on the server
/// </summary>
public class SPacketPlayersOnServer : APacketServer 
{
    public string[] Usernames { get; set; }

    public override void Write(PacketWriter writer) 
    {
        writer.Write((byte)Usernames.Length);
        foreach (var username in Usernames)
            writer.Write(username);
    }

    public override void Read(PacketReader reader)
    {
        var length = reader.ReadByte();
        Usernames = new string[length];
        for (int i = 0; i < length; i++)
            Usernames[i] = reader.ReadString();
    }

    public override void Handle(GameClient client) 
    {
        foreach (var username in Usernames)
            GameManager.UIPlayerList.AddPlayer(username);
    }
}