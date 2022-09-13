namespace Sankari;

public class Linker : Node
{
    [Export] public bool DeveloperMode;
    [Export] public bool PressPlayOnLaunch;
    [Export] public bool AutoHostJoin;
    [Export] public readonly NodePath NodePathTransition;

    public CanvasLayer CanvasLayer { get; private set; }
    public ConsoleManager ConsoleManager { get; private set; }
    public UIPlayerList UIPlayerList { get; private set; }
    public UIMapMenu UIMapMenu { get; private set; }

    private GameManager gameManager;
    
    public override async void _Ready()
    {
        CanvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        ConsoleManager = CanvasLayer.GetNode<ConsoleManager>("PanelContainer/Console");
        UIPlayerList = CanvasLayer.GetNode<UIPlayerList>("Player List");
        UIMapMenu = CanvasLayer.GetNode<UIMapMenu>("UIMapMenu");
        gameManager = new GameManager(this);

        if (DeveloperMode)
        {
            if (PressPlayOnLaunch)
                GameManager.Menu.PressPlay();

            if (AutoHostJoin)
            {
                if (OS.HasFeature("standalone"))
                {
                    // running in an exported build
                    GameManager.UIMapMenu.OnlineUsername = "OtherClient";
                    GameManager.UIMapMenu.Join();
                }
                else 
                {
                    // running in the editor
                    GameManager.UIMapMenu.OnlineUsername = "ImHost";
                    await GameManager.UIMapMenu.HostGame();
                }
            }
        }
    }

    public override async void _Process(float delta)
    {
        Logger.Update();
        await gameManager.Update();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("console"))
            ConsoleManager.ToggleVisibility();
    }

    /*public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey)
            Notifications.Notify(this, Event.OnKeyboardInput, inputEventKey);

        if (@event is InputEventMouseButton inputEventMouseButton)
            Notifications.Notify(this, Event.OnMouseButtonInput, inputEventMouseButton);

        if (@event is InputEventMouseMotion inputEventMouseMotion)
            Notifications.Notify(this, Event.OnMouseMotionInput, inputEventMouseMotion);

        if (@event is InputEventJoypadButton inputEventJoypadButton)
            Notifications.Notify(this, Event.OnJoypadButtonInput, inputEventJoypadButton);
    }*/

    public override async void _Notification(int what)
    {
        if (what == MainLoop.NotificationWmQuitRequest)
        {
            GetTree().AutoAcceptQuit = false;
            await Cleanup();
        }
    }

    private async Task Cleanup()
    {
        if (Logger.StillWorking())
            await Task.Delay(1);

        //ModLoader.SaveEnabled();
        //Options.SaveOptions();
        await GameManager.Net.Cleanup();
        GameManager.Tokens.Cleanup();
        GetTree().Quit();
    }
}
