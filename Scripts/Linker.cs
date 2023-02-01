namespace Sankari;

public partial class Linker : Node
{
	[Export] public bool DeveloperMode { get; set; }
	[Export] public bool LoadFirstLevel { get; set; }
	[Export] public bool PressPlayOnLaunch { get; set; }
	[Export] public bool AutoHostJoin { get; set; }
	[Export] public bool LoadTestLevel { get; set; }
	[Export] public bool InstantlyLoadLevel { get; set; }

	// net
	[Export] public Godot.Collections.Array<ServerPacketOpcode> IgnoreOpcodesFromServer { get; set; }
	[Export] public Godot.Collections.Array<ClientPacketOpcode> IgnoreOpcodesFromClient { get; set; }

	[Export] public NodePath NodePathTransition;

	public CanvasLayer CanvasLayer { get; private set; }
	public UIConsoleManager ConsoleManager { get; private set; }
	public UIPlayerList UIPlayerList { get; private set; }
	public UIMapMenu UIMapMenu { get; private set; }
	public Node SFXPlayers { get; private set; }

	private GameManager gameManager;
	
	public override async void _Ready()
	{
		SFXPlayers = GetNode<Node>("SFX Players");
		CanvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		ConsoleManager = CanvasLayer.GetNode<UIConsoleManager>("PanelContainer/Console");
		UIMapMenu = CanvasLayer.GetNode<UIMapMenu>("UIMapMenu");
		UIPlayerList = CanvasLayer.GetNode<UIPlayerList>("Player List");
		gameManager = new GameManager(this);
		UIPlayerList.SetupListeners();

		if (DeveloperMode)
		{
			if (PressPlayOnLaunch)
			{
				if (!LoadTestLevel)
				{
					GameManager.Menu.PressPlay();
				}
				else
				{
					Logger.LogWarning("Will not press play on launch because 'LoadTestLevel' was enabled");
				}
			}

			if (LoadFirstLevel)
			{
				GameManager.Menu.Hide();
				LevelManager.CurrentLevel = "Level A1";
				await LevelManager.LoadLevel(InstantlyLoadLevel);
			}

			if (LoadTestLevel)
			{
				GameManager.Menu.Hide();
				LevelManager.CurrentLevel = "Test Level";
				await LevelManager.LoadLevel(InstantlyLoadLevel);
			}

			if (AutoHostJoin)
			{
				if (GOS.IsExportedRelease())
				{
					// running in an exported build
					GOS.SetWindowTitle("OtherClient");
					GameManager.UIMapMenu.OnlineUsername = "OtherClient";
					GameManager.UIMapMenu.Join();
				}
				else
				{
					// running in the editor
					GOS.SetWindowTitle("ImHost");
					GameManager.UIMapMenu.OnlineUsername = "ImHost";
					await GameManager.UIMapMenu.HostGame();
				}
			}
		}
	}

	public override async void _Process(double delta)
	{
		Logger.Update();
		await GameManager.Update();
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
		
		if (what == NotificationWMCloseRequest)
		{
			GetTree().AutoAcceptQuit = false;
			await Cleanup();
		}
	}

	public async Task Cleanup()
	{
		if (Logger.StillWorking())
			await Task.Delay(1);

		//ModLoader.SaveEnabled();
		//Options.SaveOptions();
		await Net.Cleanup();
		Tokens.Cleanup();
		GetTree().Quit();
	}
}
