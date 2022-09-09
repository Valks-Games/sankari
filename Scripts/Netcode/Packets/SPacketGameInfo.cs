namespace Sankari;

public enum ServerGameInfo
{
    PlayerJoinLeave,
    PlayersOnServer,
    StartLevel,
    MapPosition
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

    // StartLevel
    public string LevelName { get; set; }

    // MapPosition
    public Vector2 MapPosition { get; set; }

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
            case ServerGameInfo.StartLevel:
                writer.Write(LevelName);
                break;
            case ServerGameInfo.MapPosition:
                writer.Write(MapPosition);
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
            case ServerGameInfo.StartLevel:
                LevelName = reader.ReadString();
                break;
            case ServerGameInfo.MapPosition:
                MapPosition = reader.ReadVector2();
                break;
        }
    }

    public override async Task Handle()
    {
        switch (ServerGameInfo)
        {
            case ServerGameInfo.PlayerJoinLeave:
                HandlePlayerJoinLeave();
                break;
            case ServerGameInfo.PlayersOnServer:
                HandlePlayersOnServer();
                break;
            case ServerGameInfo.StartLevel:
                await HandleStartLevel();
                break;
            case ServerGameInfo.MapPosition:
                HandleMapPosition();
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

    private async Task HandleStartLevel()
    {
        GameManager.Level.CurrentLevel = LevelName;
        await GameManager.Level.LoadLevel();
    }

    private void HandleMapPosition() 
    {
        GameManager.Map.SetPosition(MapPosition);
    }
}