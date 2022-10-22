namespace Sankari;

public partial class Map : Node
{
	public static bool HasMapLoadedBefore { get; private set; }
	public static Vector2 PrevPlayerMapIconPosition { get; set; }
	private static Sprite2D PlayerIcon { get; set; }

	public static void RememberPlayerPosition() => PrevPlayerMapIconPosition = PlayerIcon.Position;

	[Export] protected NodePath NodePathLevels { get; set; }
	[Export] protected NodePath NodePathTileMapLevelIcons { get; set; }
	[Export] protected NodePath NodePathTileMapTerrain { get; set; }
	[Export] protected NodePath NodePathPlayerIcon { get; set; }
	[Export] protected NodePath NodePathUIMapMenuScript { get; set; }
	[Export] protected NodePath NodePathCamera { get; set; }

	public Vector2 CurMapPos => PlayerIcon.Position;

	private Camera2D Camera { get; set; }
	private TileMap TileMapLevelIcons { get; set; }
	private TileMap TileMapTerrain { get; set; }
	private Node Levels { get; set; }

	private bool LoadingLevel { get; set; }

	public override void _Ready()
	{
		TileMapLevelIcons = GetNode<TileMap>(NodePathTileMapLevelIcons);
		TileMapTerrain = GetNode<TileMap>(NodePathTileMapTerrain);
		Levels = GetNode<Node>(NodePathLevels);
		PlayerIcon = GetNode<Sprite2D>(NodePathPlayerIcon);
		Camera = GetNode<Camera2D>(NodePathCamera);
		Camera.Current = true; // cameras are not set to current by default if a previous camera was set to current in a previous scene / child node

		foreach (var level in LevelManager.Levels.Values)
			if (level.Completed)
				TileMapLevelIcons.SetCell(0, (Vector2i)level.Position, 1); // remember 1 is gray circle

		if (HasMapLoadedBefore)
		{
			PlayerIcon.Position = PrevPlayerMapIconPosition;
			return;
		}
		else
			PrevPlayerMapIconPosition = PlayerIcon.Position;

		HasMapLoadedBefore = true;

		foreach (Area2D levelArea in Levels.GetChildren())
		{
			var worldPos = ((CollisionShape2D)levelArea.GetChild(0)).Position;
			var tilePos = TileMapLevelIcons.LocalToMap(worldPos);

			if (!LevelManager.Levels.ContainsKey(levelArea.Name)) // level has not been defined in LevelManager.cs
				LevelManager.Levels.Add(levelArea.Name, new Level(levelArea.Name)
				{
					Position = tilePos
				});
			else
				LevelManager.Levels[levelArea.Name].Position = tilePos;
		}
	}

	public override async void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
			GameManager.UIMapMenu.Visible = !GameManager.UIMapMenu.Visible;

		if (GameManager.UIMapMenu.Visible || GameManager.Console.Visible)
			return;

		if (Net.IsMultiplayer())
		{
			if (Net.IsHost()) // only the host can move around on the map
			{
				HandleMovement();
			}
		}
		else
		{
			// singleplayer
			HandleMovement();
		}

		if (Input.IsActionJustPressed("map_action") && !LoadingLevel)
		{
			// TODO GODOT 4 CONVERSION
			//if (tileMapLevelIcons.GetTileName(playerIcon.Position) == "uncleared")
			{
				if (Net.IsMultiplayer())
				{
					// only the host can start levels
					if (Net.IsHost())
					{
						LoadingLevel = true;

						Net.Server.SendToEveryoneButHost(ServerPacketOpcode.GameInfo, new SPacketGameInfo
						{
							ServerGameInfo = ServerGameInfo.StartLevel,
							LevelName = LevelManager.CurrentLevel
						});

						// WARN: Not a thread safe way of doing this
						Net.Server.LevelUpdateLoop.Start();

						await LevelManager.LoadLevel();
					}
				}
				else
				{
					// singleplayer
					LoadingLevel = true;
					await LevelManager.LoadLevel();
				}

				PrevPlayerMapIconPosition = PlayerIcon.Position;
			}
		}
	}

	public void SetPosition(Vector2 pos) => PlayerIcon.Position = pos;

	private void HandleMovement()
	{
		var moveOffset = 16;

		CheckMove("map_move_left", new Vector2(-moveOffset, 0));
		CheckMove("map_move_right", new Vector2(moveOffset, 0));
		CheckMove("map_move_up", new Vector2(0, -moveOffset));
		CheckMove("map_move_down", new Vector2(0, moveOffset));
	}

	private void CheckMove(string inputAction, Vector2 moveOffset)
	{
		if (Input.IsActionJustPressed(inputAction))
		{
			var currPos = PlayerIcon.Position;
			var nextPos = currPos + moveOffset;

			if (TileMapTerrain.GetTileName(nextPos) != "Path")
				return;

			//var tileIdLevelOffset = GetCurrentTileId(tileMapLevelIcons, nextPos);
			//var tileIdLevelCurr = GetCurrentTileId(tileMapLevelIcons, currPos);

			// yellow circle = 0
			// gray circle   = 1

			var nextTilePos = TileMapLevelIcons.LocalToMap(nextPos);

			//if (levelManager.LevelPositions.ContainsKey(nextTilePos) && levelManager.LevelPositions[nextTilePos].Locked)
			//    return;

			PlayerIcon.Position = nextPos;

			if (Net.IsMultiplayer() && Net.IsHost())
			{
				Net.Server.SendToEveryoneButHost(ServerPacketOpcode.GameInfo, new SPacketGameInfo
				{
					ServerGameInfo = ServerGameInfo.MapPosition,
					MapPosition = nextPos
				});
			}
		}
	}
	private void _on_Player_Area_area_entered(Area2D area)
	{
		LevelManager.CurrentLevel = area.Name;
	}
}
