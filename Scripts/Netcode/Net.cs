namespace Sankari.Netcode;

public class Net
{
    public DateTime PingSent { get; set; }
    public DisconnectOpcode DisconnectOpcode { get; set; }
    public GameClient Client { get; set; }
    public GameServer Server { get; set; }
    public bool EnetInitialized { get; }

    private readonly GodotCommands godotCmds;

    public Net()
    {
        godotCmds = new GodotCommands();

        Client = new GameClient(this, godotCmds);
        Server = new GameServer(this);

        try
        {
            EnetInitialized = ENet.Library.Initialize();
        }
        catch (DllNotFoundException)
        {
            var message = "ENet failed to initialize because enet.dll was not found. Please restart the game and make sure enet.dll is right next to the games executable. Because ENet failed to initialize multiplayer has been disabled.";
            Logger.LogWarning(message);
            GameManager.Popups.SpawnMessage(message);
            return;
        }

        if (!EnetInitialized) // probably won't get logged but lets keep it here because why not
            Logger.LogWarning("Failed to initialize ENet! Remember ENet-CSharp.dll and enet.dll are required in order for ENet to run properly!");

        var mapScript = GameManager.UIMapMenu;

        GameManager.Notifications.AddListener(GameManager.Linker, Event.OnGameClientStopped, (args) =>
        {
            GameManager.Net.Client.ExecuteCode((client) => client.TryingToConnect = false);
            mapScript.BtnJoin.Disabled = false;
            mapScript.BtnJoin.Text = "Join World";

            GameManager.UIPlayerList.RemoveAllPlayers();
        });

        GameManager.Notifications.AddListener(GameManager.Linker, Event.OnGameClientConnected, (args) =>
        {
            GameManager.Net.Client.Send(ClientPacketOpcode.GameInfo, new CPacketGameInfo
            {
                ClientGameInfo = ClientGameInfo.PlayerJoin,
                Username = mapScript.OnlineUsername,
                Host = mapScript.IsHost,
                Password = mapScript.HostPassword
            });

            GameManager.Net.Client.ExecuteCode((client) => client.TryingToConnect = false);
            mapScript.BtnJoin.Disabled = true;
            mapScript.BtnJoin.Text = "Connected";
        });
    }

    public async Task Update()
    {
        await godotCmds.Update();
    }

    public async void StartClient(string ip, ushort port, CancellationTokenSource cts)
    {
        if (!EnetInitialized)
        {
            Logger.LogWarning("Tried to start client but ENet was not initialized properly");
            return;
        }

        Client = new GameClient(this, godotCmds);
        await Client.StartAsync(ip, port, cts);
    }

    public async void StartServer(ushort port, int maxPlayers, CancellationTokenSource cts)
    {
        if (!EnetInitialized)
        {
            Logger.LogWarning("Tried to start server but ENet was not initialized properly");
            return;
        }

        Server = new GameServer(this);
        await Server.StartAsync(port, maxPlayers, cts);
    }

    public bool IsHost() => Server.IsRunning;
    public bool IsMultiplayer() => Client.IsRunning || Server.IsRunning;

    public async Task Cleanup()
    {
        if (Client.IsRunning)
            await Client.StopAsync();

        if (Server.IsRunning)
            await Server.StopAsync();

        if (EnetInitialized)
            ENet.Library.Deinitialize();
    }
}
