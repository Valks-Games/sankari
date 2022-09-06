namespace Sankari;

public class UIMapMenu : Control
{
    [Export] protected readonly NodePath NodePathNet;

    private LineEdit lineEditPort;
    private LineEdit lineEditPassword;
    private Button buttonServerToggle;

    private ushort port;

    public override void _Ready()
    {
        var net = GetNode<Node>(NodePathNet);
        lineEditPort = net.GetNode<LineEdit>("Port");
        lineEditPassword = net.GetNode<LineEdit>("Password");
        buttonServerToggle = net.GetNode<Button>("Server Toggle");

        port = ushort.Parse(lineEditPort.Text);

        Hide();
    }

    private void _on_Port_text_changed(string text)
    {
        port = (ushort)lineEditPort.FilterRange(ushort.MaxValue);
    }

    private void _on_Password_text_changed(string text)
    {
        
    }

    private async void _on_Server_Toggle_pressed() 
    {
        if (GameManager.Net.Server.IsRunning)
        {
            GameManager.Net.Server.Stop();
            GameManager.Net.Client.Stop();

            buttonServerToggle.Text = "Open World to Other Players";
        }
        else 
        {
            var ctsServer = GameManager.Tokens.Create("server_running");
            var ctsClient = GameManager.Tokens.Create("client_running");

            var net = GameManager.Net;

            net.StartServer(port, 10, ctsServer);
            net.StartClient("127.0.0.1", port, ctsClient);

            while (!net.Server.HasSomeoneConnected)
                await Task.Delay(1);

            net.Client.Send(ClientPacketOpcode.PlayerJoinServer, new CPacketPlayerJoinServer {
                Username = "Player",
                Host = true,
                Password = "123"
            });

            buttonServerToggle.Text = "Close World to Other Players";
        }
    }

    private void _on_Back_to_Main_Menu_pressed()
    {
        GameManager.LevelUI.Hide();
        Map.RememberPlayerPosition();
        GameManager.DestroyMap();
        GameManager.ShowMenu();
    }
}
