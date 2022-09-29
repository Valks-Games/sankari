namespace Sankari;

public partial class UIMapMenu : Control
{
    [Export] protected NodePath NodePathHost { get; set; }
    [Export] protected NodePath NodePathJoin { get; set; }
    [Export] protected NodePath NodePathOnlineUsername { get; set; }

    public Button BtnJoin { get; private set; }
    public bool IsHost { get; private set; }
    public string OnlineUsername { get; set; } = "";
    public string HostPassword { get; set; } = "";

    private Control ControlHost { get; set; }
    private LineEdit LineEditHostPort { get; set; }
    private LineEdit LineEditHostPassword { get; set; }
    private Button BtnHostServerToggle { get; set; }

    private Control ControlJoin { get; set; }
    private LineEdit LineEditJoinIp { get; set; }
    private LineEdit LineEditJoinPassword { get; set; }

    private LineEdit LineEditOnlineUsername { get; set; }


    private ushort HostPort { get; set; }

    private string JoinIP { get; set; } = "";
    private string JoinPassword { get; set; } = "";

    public override void _Ready()
    {
        ControlHost = GetNode<Control>(NodePathHost);
        ControlJoin = GetNode<Control>(NodePathJoin);

        LineEditHostPort = ControlHost.GetNode<LineEdit>("Port");
        LineEditHostPassword = ControlHost.GetNode<LineEdit>("Password");
        BtnHostServerToggle = ControlHost.GetNode<Button>("Server Toggle");

        LineEditJoinIp = ControlJoin.GetNode<LineEdit>("IP");
        LineEditJoinPassword = ControlJoin.GetNode<LineEdit>("Join Password");
        BtnJoin = ControlJoin.GetNode<Button>("Join World3D");

        LineEditOnlineUsername = GetNode<LineEdit>(NodePathOnlineUsername);
        OnlineUsername = LineEditOnlineUsername.Text;

        ControlHost.Show();
        ControlJoin.Hide();

        HostPort = ushort.Parse(LineEditHostPort.Text);
        JoinIP = LineEditJoinIp.Text;

        Hide();
    }

    private bool InvalidOnlineUsername() => string.IsNullOrWhiteSpace(OnlineUsername);

    // host
    private void _on_Host_pressed() 
    {
        ControlHost.Show();
        ControlJoin.Hide();
    }

    private void _on_Port_text_changed(string text) =>
        HostPort = (ushort)LineEditHostPort.FilterRange(ushort.MaxValue);

    private void _on_Password_text_changed(string text) => HostPassword = text;

    private async void _on_Server_Toggle_pressed() 
    {
        if (InvalidOnlineUsername()) 
        {
            Popups.SpawnMessage("Please provide a valid online username");
            return;
        }

        if (Net.Server.IsRunning)
        {
            Net.Server.Stop();
            Net.Client.Stop();

            BtnHostServerToggle.Text = "Open World3D to Other Players";
        }
        else 
        {
            await HostGame("127.0.0.1", HostPort, 10);
        }
    }

    public async Task HostGame(string ip = "127.0.0.1", ushort port = 25565, int maxPlayers = 10)
    {
        var ctsServer = Tokens.Create("server_running");
        var ctsClient = Tokens.Create("client_running");

        IsHost = true;

        Net.StartServer(HostPort, maxPlayers, ctsServer);
        Net.StartClient(ip, port, ctsClient); // TODO: Get external IP automatically

        while (!Net.Server.HasSomeoneConnected)
            await Task.Delay(1);

        BtnHostServerToggle.Text = "Close World3D to Other Players";
    }

    // join
    private void _on_Join_pressed() 
    {
        ControlJoin.Show();
        ControlHost.Hide();
    }

    private void _on_IP_text_changed(string text) => JoinIP = text;

    private void _on_Join_Password_text_changed(string text) => JoinPassword = text;

    private void _on_Online_Username_text_changed(string text) => OnlineUsername = text;

    private void _on_Join_World_pressed()
    {
        if (InvalidOnlineUsername()) 
        {
            Popups.SpawnMessage("Please provide a valid online username");
            return;
        }

        // WARN: Not thread safe to access net.Client.TryingToConnect directly from another thread
        // Note: This should not cause any problems
        if (Net.Client.TryingToConnect || Net.Client.IsConnected) 
            return;

        var indexColon = JoinIP.IndexOf(":");

        if (indexColon == -1) 
        {
            Popups.SpawnMessage("The address is missing a port");
            return;
        }

        var address = JoinIP.Substring(0, indexColon);
        var port = JoinIP.Substring(indexColon + 1);

        if (!address.IsAddress() || string.IsNullOrWhiteSpace(address)) 
        {
            Popups.SpawnMessage("Please enter a valid address");
            return;
        }

        if (string.IsNullOrWhiteSpace(port) || !ushort.TryParse(port, out ushort portNum)) 
        {
            Popups.SpawnMessage("Please enter a valid port");
            return;
        }

        Join(address, portNum);
    }

    public void Join(string ip = "127.0.0.1", ushort port = 25565)
    {
        IsHost = false;

        Net.Client.ExecuteCode((client) => client.TryingToConnect = true);
        BtnJoin.Disabled = true;
        BtnJoin.Text = "Searching for world...";
        var ctsClient = Tokens.Create("client_running");
        Net.StartClient(ip, port, ctsClient);
    }

    // game
    private void _on_Back_to_Main_Menu_pressed()
    {
        if (Net.Server.IsRunning)
            Net.Server.Stop();

        if (Net.Client.IsRunning)
            Net.Client.Stop();

        GameManager.LevelUI.Hide();
        Map.RememberPlayerPosition();
        GameManager.DestroyMap();
        GameManager.ShowMenu();

        Hide();
    }
}
