namespace Sankari.Netcode;

public static class Net
{
    public static DateTime PingSent { get; set; }
    public static DisconnectOpcode DisconnectOpcode { get; set; }
    public static GameClient Client { get; set; }
    public static GameServer Server { get; set; }
    public static bool EnetInitialized { get; set; }

    public static void Init()
    {
        Client = new GameClient();
        Server = new GameServer();

        try
        {
            EnetInitialized = ENet.Library.Initialize();
        }
        catch (DllNotFoundException)
        {
            var message = "ENet failed to initialize because enet.dll was not found. Please restart the game and make sure enet.dll is right next to the games executable. Because ENet failed to initialize multiplayer has been disabled.";
            Logger.LogWarning(message);
            Popups.SpawnMessage(message);
            return;
        }

        if (!EnetInitialized) // probably won't get logged but lets keep it here because why not
            Logger.LogWarning("Failed to initialize ENet! Remember ENet-CSharp.dll and enet.dll are required in order for ENet to run properly!");

        var mapScript = GameManager.UIMapMenu;

        GameManager.Events.AddListener(GameManager.Linker, Event.OnGameClientStopped, (args) =>
        {
            Client.ExecuteCode((client) => client.TryingToConnect = false);
            mapScript.BtnJoin.Disabled = false;
            mapScript.BtnJoin.Text = "Join World3D";

            GameManager.UIPlayerList.RemoveAllPlayers();
        });

        GameManager.Events.AddListener(GameManager.Linker, Event.OnGameClientConnected, (args) =>
        {
            Client.Send(ClientPacketOpcode.GameInfo, new CPacketGameInfo
            {
                ClientGameInfo = ClientGameInfo.PlayerJoin,
                Username = mapScript.OnlineUsername,
                Host = mapScript.IsHost,
                Password = mapScript.HostPassword
            });

            Client.ExecuteCode((client) => client.TryingToConnect = false);
            mapScript.BtnJoin.Disabled = true;
            mapScript.BtnJoin.Text = "Connected";
        });
    }

    public static async Task Update()
    {
        await GodotCommands.Update();
    }

    public static async void StartClient(string ip, ushort port, CancellationTokenSource cts)
    {
        if (!EnetInitialized)
        {
            Logger.LogWarning("Tried to start client but ENet was not initialized properly");
            return;
        }

        Client = new GameClient();
        await Client.StartAsync(ip, port, cts);
    }

    public static async void StartServer(ushort port, int maxPlayers, CancellationTokenSource cts)
    {
        if (!EnetInitialized)
        {
            Logger.LogWarning("Tried to start server but ENet was not initialized properly");
            return;
        }

        Server = new GameServer();
        await Server.StartAsync(port, maxPlayers, cts);
    }

    public static bool IsHost() => Server.IsRunning;
    public static bool IsMultiplayer() => Client.IsRunning || Server.IsRunning;

    public static async Task Cleanup()
    {
        if (Client.IsRunning)
            await Client.StopAsync();

        if (Server.IsRunning)
            await Server.StopAsync();

        if (EnetInitialized)
            ENet.Library.Deinitialize();
    }
}
