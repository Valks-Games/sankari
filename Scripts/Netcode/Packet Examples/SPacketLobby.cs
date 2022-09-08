using Sankari.Netcode.Client;

namespace Sankari.Netcode;

public class SPacketLobby : APacketServerPeerId
{
    private LobbyOpcode LobbyOpcode { get; set; }

    // LobbyMessage
    public string Message { get; set; }

    // LobbyCountdownChange
    public bool CountdownRunning { get; set; }

    // LobbyInfo
    public Dictionary<byte, DataPlayer> Players { get; set; }
    public bool DirectConnect { get; set; }
    public string LobbyName { get; set; }
    public string LobbyDescription { get; set; }
    public ushort LobbyMaxPlayerCount { get; set; }
    public byte LobbyHostId { get; set; }

    // LobbyJoin
    public string Username { get; set; }

    // LobbyReady
    public bool Ready { get; set; }

    public SPacketLobby() { } // required because of ReflectionUtils

    public SPacketLobby(LobbyOpcode opcode)
    {
        LobbyOpcode = opcode;
    }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)LobbyOpcode);

        switch (LobbyOpcode)
        {
            case LobbyOpcode.LobbyCreate:
                base.Write(writer);
                break;

            case LobbyOpcode.LobbyChatMessage:
                base.Write(writer);
                writer.Write((string)Message);
                break;

            case LobbyOpcode.LobbyCountdownChange:
                writer.Write(CountdownRunning);
                break;

            case LobbyOpcode.LobbyInfo:
                base.Write(writer);
                writer.Write((byte)Players.Count);
                Players.ForEach(pair =>
                {
                    writer.Write((byte)pair.Key); // id
                    writer.Write((string)pair.Value.Username);
                });
                writer.Write(DirectConnect);
                if (DirectConnect)
                {
                    writer.Write(LobbyName);
                    writer.Write(LobbyDescription);
                    writer.Write(LobbyHostId);
                    writer.Write(LobbyMaxPlayerCount);
                }
                break;

            case LobbyOpcode.LobbyJoin:
                base.Write(writer);
                writer.Write((string)Username);
                break;

            case LobbyOpcode.LobbyLeave:
                base.Write(writer);
                break;

            case LobbyOpcode.LobbyReady:
                base.Write(writer);
                writer.Write(Ready);
                break;
        }
    }

    public override void Read(PacketReader reader)
    {
        LobbyOpcode = (LobbyOpcode)reader.ReadByte();

        switch (LobbyOpcode)
        {
            case LobbyOpcode.LobbyCreate:
                base.Read(reader);
                break;

            case LobbyOpcode.LobbyChatMessage:
                base.Read(reader);
                Message = reader.ReadString();
                break;

            case LobbyOpcode.LobbyCountdownChange:
                CountdownRunning = reader.ReadBool();
                break;

            case LobbyOpcode.LobbyInfo:
                base.Read(reader);
                var count = reader.ReadByte();
                Players = new Dictionary<byte, DataPlayer>();
                for (int i = 0; i < count; i++)
                {
                    var id = reader.ReadByte();
                    var name = reader.ReadString();

                    Players[id] = new DataPlayer
                    {
                        Username = name,
                        Ready = false
                    };
                }
                var directConnected = reader.ReadBool();
                if (directConnected)
                {
                    LobbyName = reader.ReadString();
                    LobbyDescription = reader.ReadString();
                    LobbyHostId = reader.ReadByte();
                    LobbyMaxPlayerCount = reader.ReadUShort();
                }
                break;

            case LobbyOpcode.LobbyJoin:
                base.Read(reader);
                Username = reader.ReadString();
                break;

            case LobbyOpcode.LobbyLeave:
                base.Read(reader);
                break;

            case LobbyOpcode.LobbyReady:
                base.Read(reader);
                Ready = reader.ReadBool();
                break;
        }
    }

#if CLIENT
    public override async Task Handle(GameClient client, Managers managers)
    {
        switch (LobbyOpcode)
        {
            case LobbyOpcode.LobbyCreate:
                await HandleCreate(managers);
                break;

            case LobbyOpcode.LobbyInfo:
                await HandleInfo();
                break;

            case LobbyOpcode.LobbyJoin:
                HandleJoin(managers);
                break;

            case LobbyOpcode.LobbyChatMessage:
                HandleChatMessage();
                break;

            case LobbyOpcode.LobbyLeave:
                HandleLeave();
                break;

            case LobbyOpcode.LobbyReady:
                HandleReady();
                break;

            case LobbyOpcode.LobbyCountdownChange:
                HandleCountdownChange();
                break;

            case LobbyOpcode.LobbyGameStart:
                await HandleGameStart();
                break;
        }
    }

    private async Task HandleCreate(Managers managers)
    {
        /*NetworkManager.PeerId = Id;
        NetworkManager.IsHost = true;
        Client.Log($"{Logger.Options.OnlineUsername} created lobby with their id being {Id}");
        Client.Players[Id] = Logger.Options.OnlineUsername;

        await SceneManager.ChangeScene("Lobby");*/

        await managers.ManagerScene.ChangeScene(GameScene.Lobby);
    }

    private void HandleChatMessage()
    {
        /*if (SceneManager.InLobby())
            SceneManager.GetActiveSceneScript<SceneLobby>().LobbyChat.Print(Id, Message);*/
    }

    private void HandleCountdownChange()
    {
        /*if (!SceneManager.InLobby())
            return;

        if (CountdownRunning)
            SceneManager.GetActiveSceneScript<SceneLobby>().StartGameCountdown();
        else
            SceneManager.GetActiveSceneScript<SceneLobby>().CancelGameCountdown();*/
    }

    private async Task HandleGameStart()
    {
        /*if (NetworkManager.IsHost)
            NetworkManager.GameServer.ENetCmds.Enqueue(new ThreadCmd<ENetOpcode>(ENetOpcode.StartGame));

        await SceneManager.ChangeScene("Game");*/
        await Task.FromResult(1);
    }

    private async Task HandleInfo()
    {
        /*NetworkManager.PeerId = Id;
        Client.Log($"{Logger.Options.OnlineUsername} joined lobby with id {Id}");
        Client.Players[Id] = Logger.Options.OnlineUsername;
        Players.ForEach(pair => NetworkManager.GameClient.Players[pair.Key] = pair.Value.Username);

        var currentLobby = NetworkManager.CurrentLobby;
        currentLobby.Name = LobbyName;
        currentLobby.Description = LobbyDescription;
        currentLobby.LobbyHostId = LobbyHostId;
        currentLobby.MaxPlayerCount = LobbyMaxPlayerCount;

        await SceneManager.ChangeScene("Lobby");*/
        await Task.FromResult(1);
    }

    private void HandleJoin(Managers managers)
    {
        var lobbyScene = (SceneLobby)managers.ManagerScene.ActiveScene;
        lobbyScene.AddPlayerListing("Bob");

        /*if (SceneManager.InLobby())
            SceneManager.GetActiveSceneScript<SceneLobby>().AddPlayer(Id, Username);
        Client.Players[Id] = Username;
        Client.Log($"Player with username {Username} id: {Id} joined the lobby");*/
    }

    private void HandleLeave()
    {
        /*if (SceneManager.InLobby())
            SceneManager.GetActiveSceneScript<SceneLobby>().RemovePlayer(Id);

        if (SceneManager.InGame())
            SceneManager.GetActiveSceneScript<Game.SceneGame>().RemovePlayer(Id);

        Client.Players.Remove(Id);
        Client.Log($"Player with id: {Id} left the lobby");*/
    }

    private void HandleReady()
    {
        /*if (SceneManager.InLobby())
            SceneManager.GetActiveSceneScript<SceneLobby>().SetReady(Id, Ready);*/
    }
#endif
}
