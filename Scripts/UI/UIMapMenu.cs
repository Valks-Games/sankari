namespace Sankari;

public class UIMapMenu : Control
{
    [Export] protected readonly NodePath NodePathHost;
    [Export] protected readonly NodePath NodePathJoin;
    [Export] protected readonly NodePath NodePathOnlineUsername;

    public Button BtnJoin { get; private set; }
    public bool IsHost { get; private set; }
    public string OnlineUsername { get; set; } = "";
    public string HostPassword { get; set; } = "";

    private Control controlHost;
    private LineEdit lineEditHostPort;
    private LineEdit lineEditHostPassword;
    private Button btnHostServerToggle;

    private Control controlJoin;
    private LineEdit lineEditJoinIp;
    private LineEdit lineEditJoinPassword;

    private LineEdit lineEditOnlineUsername;


    private ushort hostPort;

    private string joinIP = "";
    private string joinPassword = "";

    public override void _Ready()
    {
        controlHost = GetNode<Control>(NodePathHost);
        controlJoin = GetNode<Control>(NodePathJoin);

        lineEditHostPort = controlHost.GetNode<LineEdit>("Port");
        lineEditHostPassword = controlHost.GetNode<LineEdit>("Password");
        btnHostServerToggle = controlHost.GetNode<Button>("Server Toggle");

        lineEditJoinIp = controlJoin.GetNode<LineEdit>("IP");
        lineEditJoinPassword = controlJoin.GetNode<LineEdit>("Join Password");
        BtnJoin = controlJoin.GetNode<Button>("Join World");

        lineEditOnlineUsername = GetNode<LineEdit>(NodePathOnlineUsername);
        OnlineUsername = lineEditOnlineUsername.Text;

        controlHost.Show();
        controlJoin.Hide();

        hostPort = ushort.Parse(lineEditHostPort.Text);
        joinIP = lineEditJoinIp.Text;

        Hide();
    }

    private bool InvalidOnlineUsername() => string.IsNullOrWhiteSpace(OnlineUsername);

    // host
    private void _on_Host_pressed() 
    {
        controlHost.Show();
        controlJoin.Hide();
    }

    private void _on_Port_text_changed(string text) =>
        hostPort = (ushort)lineEditHostPort.FilterRange(ushort.MaxValue);

    private void _on_Password_text_changed(string text) => HostPassword = text;

    private async void _on_Server_Toggle_pressed() 
    {
        if (InvalidOnlineUsername()) 
        {
            GameManager.Popups.SpawnMessage("Please provide a valid online username");
            return;
        }

        if (GameManager.Net.Server.IsRunning)
        {
            GameManager.Net.Server.Stop();
            GameManager.Net.Client.Stop();

            btnHostServerToggle.Text = "Open World to Other Players";
        }
        else 
        {
            await HostGame("127.0.0.1", hostPort, 10);
        }
    }

    public async Task HostGame(string ip = "127.0.0.1", ushort port = 25565, int maxPlayers = 10)
    {
        var ctsServer = GameManager.Tokens.Create("server_running");
        var ctsClient = GameManager.Tokens.Create("client_running");

        var net = GameManager.Net;

        IsHost = true;

        net.StartServer(hostPort, maxPlayers, ctsServer);
        net.StartClient(ip, port, ctsClient); // TODO: Get external IP automatically

        while (!net.Server.HasSomeoneConnected)
            await Task.Delay(1);

        btnHostServerToggle.Text = "Close World to Other Players";
    }

    // join
    private void _on_Join_pressed() 
    {
        controlJoin.Show();
        controlHost.Hide();
    }

    private void _on_IP_text_changed(string text) => joinIP = text;

    private void _on_Join_Password_text_changed(string text) => joinPassword = text;

    private void _on_Online_Username_text_changed(string text) => OnlineUsername = text;

    private void _on_Join_World_pressed()
    {
        if (InvalidOnlineUsername()) 
        {
            GameManager.Popups.SpawnMessage("Please provide a valid online username");
            return;
        }

        var net = GameManager.Net;

        if (net.Client.TryingToConnect || net.Client.IsConnected) 
            return;

        var popups = GameManager.Popups;

        var indexColon = joinIP.IndexOf(":");

        if (indexColon == -1) 
        {
            popups.SpawnMessage("The address is missing a port");
            return;
        }

        var address = joinIP.Substring(0, indexColon);
        var port = joinIP.Substring(indexColon + 1);

        if (!address.IsAddress() || string.IsNullOrWhiteSpace(address)) 
        {
            popups.SpawnMessage("Please enter a valid address");
            return;
        }

        if (string.IsNullOrWhiteSpace(port) || !ushort.TryParse(port, out ushort portNum)) 
        {
            popups.SpawnMessage("Please enter a valid port");
            return;
        }

        Join(address, portNum);
    }

    public void Join(string ip = "127.0.0.1", ushort port = 25565)
    {
        IsHost = false;

        GameManager.Net.Client.TryingToConnect = true;
        BtnJoin.Disabled = true;
        BtnJoin.Text = "Searching for world...";
        var ctsClient = GameManager.Tokens.Create("client_running");
        GameManager.Net.StartClient(ip, port, ctsClient);
    }

    // game
    private void _on_Back_to_Main_Menu_pressed()
    {
        if (GameManager.Net.Server.IsRunning)
            GameManager.Net.Server.Stop();

        if (GameManager.Net.Client.IsRunning)
            GameManager.Net.Client.Stop();

        GameManager.LevelUI.Hide();
        Map.RememberPlayerPosition();
        GameManager.DestroyMap();
        GameManager.ShowMenu();

        Hide();
    }
}
