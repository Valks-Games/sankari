namespace Sankari;

public enum ServerGameInfo
{
    PlayerJoinLeave,
    PlayersOnServer
}

/// <summary>
/// Tell a client that this person is playing with us.
/// </summary>
public class SPacketGameInfo : APacketServer
{
    public ServerGameInfo ServerGameInfo { get; set; }

    // PlayerJoinLeave
    public string Username { get; set; }
    public bool Joining { get; set; }

    // PlayersOnServer
    public string[] Usernames { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((ushort)ServerGameInfo);

        switch (ServerGameInfo)
        {
            case ServerGameInfo.PlayerJoinLeave:
                writer.Write(Username);
                writer.Write(Joining);
                break;
            case ServerGameInfo.PlayersOnServer:
                writer.Write((byte)Usernames.Length);
                foreach (var username in Usernames)
                    writer.Write(username);
                break;
        }
    }

    public override void Read(PacketReader reader)
    {
        ServerGameInfo = (ServerGameInfo)reader.ReadUShort();

        Logger.Log($"[Client] Received: {ServerGameInfo}");

        switch (ServerGameInfo)
        {
            case ServerGameInfo.PlayerJoinLeave:
                Username = reader.ReadString();
                Joining = reader.ReadBool();
                break;
            case ServerGameInfo.PlayersOnServer:
                var length = reader.ReadByte();
                Usernames = new string[length];
                for (int i = 0; i < length; i++)
                    Usernames[i] = reader.ReadString();
                break;
        }
    }

    public override void Handle()
    {
        switch (ServerGameInfo)
        {
            case ServerGameInfo.PlayerJoinLeave:
                HandlePlayerJoinLeave();
                break;
            case ServerGameInfo.PlayersOnServer:
                break;
        }
    }

    private void HandlePlayerJoinLeave()
    {
        if (Joining)
            GameManager.UIPlayerList.AddPlayer(Username);
        else
            GameManager.UIPlayerList.RemovePlayer(Username);
    }

    private void HandlePlayersOnServer() 
    {
        foreach (var username in Usernames)
            GameManager.UIPlayerList.AddPlayer(username);
    }
}