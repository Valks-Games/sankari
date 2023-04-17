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
        BtnJoin = ControlJoin.GetNode<Button>("Join World");

        LineEditOnlineUsername = GetNode<LineEdit>(NodePathOnlineUsername);
        OnlineUsername = LineEditOnlineUsername.Text;

        ControlHost.Show();
        ControlJoin.Hide();

        HostPort = ushort.Parse(LineEditHostPort.Text);
        JoinIP = LineEditJoinIp.Text;

        Hide();
    }

    private bool InvalidOnlineUsername() => string.IsNullOrWhiteSpace(OnlineUsername);

    // join
    private void _on_Join_pressed() 
    {
        ControlJoin.Show();
        ControlHost.Hide();
    }

    private void _on_IP_text_changed(string text) => JoinIP = text;

    private void _on_Join_Password_text_changed(string text) => JoinPassword = text;

    private void _on_Online_Username_text_changed(string text) => OnlineUsername = text;

    // game
    private void _on_Back_to_Main_Menu_pressed()
    {
        GameManager.LevelUI.Hide();
        Map.RememberPlayerPosition();
        GameManager.DestroyMap();
        GameManager.ShowMenu();

        Hide();
    }

    // host
    private void _on_Host_pressed() 
    {
        ControlHost.Show();
        ControlJoin.Hide();
    }

    private void _on_Port_text_changed(string text) =>
        HostPort = (ushort)LineEditHostPort.FilterRange(ushort.MaxValue);

    private void _on_Password_text_changed(string text) => HostPassword = text;
}
