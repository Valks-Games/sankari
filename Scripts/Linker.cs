namespace Sankari;

public partial class Linker : Node
{
	[Export] public bool DeveloperMode { get; set; }
	[Export] public bool InstantlyLoadLevel1 { get; set; }
	[Export] public bool PressPlayOnLaunch { get; set; }
	[Export] public bool AutoHostJoin { get; set; }

	// net
	//[Export] public ServerPacketOpcode[] IgnoreOpcodesFromServer; // TODO: Convert to Godot4
	//[Export] public ClientPacketOpcode[] IgnoreOpcodesFromClient; // TODO: Convert to Godot4

	[Export] public NodePath NodePathTransition;

	public CanvasLayer CanvasLayer { get; private set; }
	public UIConsoleManager ConsoleManager { get; private set; }
	public UIPlayerList UIPlayerList { get; private set; }
	public UIMapMenu UIMapMenu { get; private set; }

	private GameManager gameManager;
	
	public override async void _Ready()
	{
		CanvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		ConsoleManager = CanvasLayer.GetNode<UIConsoleManager>("PanelContainer/Console");
		UIMapMenu = CanvasLayer.GetNode<UIMapMenu>("UIMapMenu");
		UIPlayerList = CanvasLayer.GetNode<UIPlayerList>("Player List");
		gameManager = new GameManager(this);
		UIPlayerList.SetupListeners();

		if (DeveloperMode)
		{
			if (PressPlayOnLaunch)
				GameManager.Menu.PressPlay();

			if (InstantlyLoadLevel1)
			{
				GameManager.Menu.Hide();
				GameManager.Level.CurrentLevel = "Level A1";
				await GameManager.Level.LoadLevel();
			}

			if (AutoHostJoin)
			{
				if (OS.HasFeature("standalone"))
				{
					// running in an exported build
					//OS.SetWindowTitle("OtherClient"); // TODO: Godot 4 conversion
					//GameManager.UIMapMenu.OnlineUsername = "OtherClient";
					//GameManager.UIMapMenu.Join();
				}
				else 
				{
					// running in the editor
					//OS.SetWindowTitle("ImHost"); // TODO: Godot 4 conversion
					//GameManager.UIMapMenu.OnlineUsername = "ImHost";
					//await GameManager.UIMapMenu.HostGame();
				}
			}
		}
	}

	public override async void _Process(double delta)
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

	public override async void _Notification(long what)
	{
		if (what == (int)NotificationWmCloseRequest)
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
