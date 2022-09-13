namespace Sankari;

public enum ServerGameInfo
{
    PlayerJoinLeave,
    PlayersOnServer,
    StartLevel,
    MapPosition,
    PeerId
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
    public byte Id { get; set; }

    // PlayersOnServer
    public Dictionary<byte, string> Usernames { get; set; }

    // StartLevel
    public string LevelName { get; set; }

    // MapPosition
    public Vector2 MapPosition { get; set; }

    // PeerId
    public byte PeerId { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((ushort)ServerGameInfo);

        switch (ServerGameInfo)
        {
            case ServerGameInfo.PlayerJoinLeave:
                writer.Write(Username);
                writer.Write(Joining);
                writer.Write((byte)Id);
                break;
            case ServerGameInfo.PlayersOnServer:
                writer.Write((byte)Usernames.Count);
                foreach (var player in Usernames) 
                {
                    writer.Write((byte)player.Key);
                    writer.Write(player.Value);
                }
                break;
            case ServerGameInfo.StartLevel:
                writer.Write(LevelName);
                break;
            case ServerGameInfo.MapPosition:
                writer.Write(MapPosition);
                break;
            case ServerGameInfo.PeerId:
                writer.Write((byte)PeerId);
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
                Id = reader.ReadByte();
                break;
            case ServerGameInfo.PlayersOnServer:
                var length = reader.ReadByte();
                Usernames = new Dictionary<byte, string>();
                for (int i = 0; i < length; i++) 
                {
                    var key = reader.ReadByte();
                    var value = reader.ReadString();

                    Usernames.Add(key, value);
                }
                break;
            case ServerGameInfo.StartLevel:
                LevelName = reader.ReadString();
                break;
            case ServerGameInfo.MapPosition:
                MapPosition = reader.ReadVector2();
                break;
            case ServerGameInfo.PeerId:
                PeerId = reader.ReadByte();
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
            case ServerGameInfo.PeerId:
                HandlePeerId();
                break;
        }
    }

    private void HandlePlayerJoinLeave()
    {
        GameManager.Notifications.Notify(Event.OnGameClientLeft, Id);

        if (Joining)
            GameManager.UIPlayerList.AddPlayer(Id, Username);
        else
            GameManager.UIPlayerList.RemovePlayer(Id);
    }

    private void HandlePlayersOnServer() 
    {
        foreach (var player in Usernames)
            GameManager.UIPlayerList.AddPlayer(player.Key, player.Value);
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

    private void HandlePeerId()
    {
        GameManager.Net.Client.PeerId = PeerId;
    }
}
