namespace Sankari;

public class UIMapMenu : Control
{
    [Export] protected readonly NodePath NodePathHost;
    [Export] protected readonly NodePath NodePathJoin;

    private Control controlHost;
    private LineEdit lineEditHostPort;
    private LineEdit lineEditHostPassword;
    private Button btnHostServerToggle;

    private Control controlJoin;
    private LineEdit lineEditJoinIp;
    private LineEdit lineEditJoinPassword;
    private Button btnJoin;

    private ushort hostPort;
    private string hostPassword = "";

    private string ip = "";
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
        btnJoin = controlJoin.GetNode<Button>("Join World");

        controlHost.Show();
        controlJoin.Hide();

        hostPort = ushort.Parse(lineEditHostPort.Text);

        Hide();
    }

    // host
    private void _on_Host_pressed() 
    {
        controlHost.Show();
        controlJoin.Hide();
    }

    private void _on_Port_text_changed(string text) =>
        hostPort = (ushort)lineEditHostPort.FilterRange(ushort.MaxValue);

    private void _on_Password_text_changed(string text) => hostPassword = text;

    private async void _on_Server_Toggle_pressed() 
    {
        if (GameManager.Net.Server.IsRunning)
        {
            GameManager.Net.Server.Stop();
            GameManager.Net.Client.Stop();

            btnHostServerToggle.Text = "Open World to Other Players";
        }
        else 
        {
            var ctsServer = GameManager.Tokens.Create("server_running");
            var ctsClient = GameManager.Tokens.Create("client_running");

            var net = GameManager.Net;

            net.StartServer(hostPort, 10, ctsServer);
            net.StartClient("127.0.0.1", hostPort, ctsClient); // TODO: Get external IP automatically

            while (!net.Server.HasSomeoneConnected)
                await Task.Delay(1);

            net.Client.Send(ClientPacketOpcode.PlayerJoinServer, new CPacketPlayerJoinServer {
                Username = "Player",
                Host = true,
                Password = hostPassword
            });

            btnHostServerToggle.Text = "Close World to Other Players";
        }
    }

    // join
    private void _on_Join_pressed() 
    {
        controlJoin.Show();
        controlHost.Hide();
    }

    private void _on_IP_text_changed(string text) => ip = text;

    private void _on_Join_Password_text_changed(string text) => joinPassword = text;

    private void _on_Join_World_pressed()
    {
        var net = GameManager.Net;

        if (net.Client.TryingToConnect) 
        {
            return;
        }

        var popups = GameManager.Popups;

        var indexColon = ip.IndexOf(":");

        if (indexColon == -1) 
        {
            popups.SpawnMessage("The address is missing a port");
            return;
        }

        var address = ip.Substring(0, indexColon);
        var port = ip.Substring(indexColon + 1);

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

        net.Client.TryingToConnect = true;
        btnJoin.Disabled = true;
        btnJoin.Text = "Searching for world...";
        var ctsClient = GameManager.Tokens.Create("client_running");
        net.StartClient(address, portNum, ctsClient);
    }

    // game
    private void _on_Back_to_Main_Menu_pressed()
    {
        GameManager.LevelUI.Hide();
        Map.RememberPlayerPosition();
        GameManager.DestroyMap();
        GameManager.ShowMenu();
    }
}
