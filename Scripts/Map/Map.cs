namespace Sankari;

public class Map : Node
{
    public static bool HasMapLoadedBefore { get; private set; }
    public static Vector2 PrevPlayerMapIconPosition { get; set; }
    private static Sprite playerIcon;

    public static void RememberPlayerPosition() => PrevPlayerMapIconPosition = playerIcon.Position;

    [Export] protected readonly NodePath NodePathLevels;
    [Export] protected readonly NodePath NodePathTileMapLevelIcons;
    [Export] protected readonly NodePath NodePathTileMapTerrain;
    [Export] protected readonly NodePath NodePathPlayerIcon;
    [Export] protected readonly NodePath NodePathUIMapMenuScript;

    private TileMap tileMapLevelIcons, tileMapTerrain;
    private Node levels;

    private bool loadingLevel;

    public override void _Ready()
    {
        tileMapLevelIcons = GetNode<TileMap>(NodePathTileMapLevelIcons);
        tileMapTerrain = GetNode<TileMap>(NodePathTileMapTerrain);
        levels = GetNode<Node>(NodePathLevels);
        playerIcon = GetNode<Sprite>(NodePathPlayerIcon);

        foreach (var level in GameManager.Level.Levels.Values)
            if (level.Completed)
                tileMapLevelIcons.SetCellv(level.Position, 1); // remember 1 is gray circle

        if (HasMapLoadedBefore)
        {
            playerIcon.Position = PrevPlayerMapIconPosition;
            return;
        }
        else
            PrevPlayerMapIconPosition = playerIcon.Position;

        HasMapLoadedBefore = true;

        foreach (Area2D levelArea in levels.GetChildren())
        {
            var worldPos = ((CollisionShape2D)levelArea.GetChild(0)).Position;
            var tilePos = tileMapLevelIcons.WorldToMap(worldPos);

            if (!GameManager.Level.Levels.ContainsKey(levelArea.Name)) // level has not been defined in LevelManager.cs
                GameManager.Level.Levels.Add(levelArea.Name, new Level(levelArea.Name)
                {
                    Position = tilePos
                });
            else
                GameManager.Level.Levels[levelArea.Name].Position = tilePos;
        }
    }

    public override async void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
            GameManager.UIMapMenu.Visible = !GameManager.UIMapMenu.Visible;

        if (GameManager.UIMapMenu.Visible)
            return;

        CheckMove("map_move_left", new Vector2(-16, 0));
        CheckMove("map_move_right", new Vector2(16, 0));
        CheckMove("map_move_up", new Vector2(0, -16));
        CheckMove("map_move_down", new Vector2(0, 16));

        if (Input.IsActionJustPressed("map_action") && !loadingLevel)
        {
            if (tileMapLevelIcons.GetTileName(playerIcon.Position) == "uncleared")
            {
                loadingLevel = true;
                await GameManager.Transition.AlphaToBlackAndBack();

                GameManager.Level.LoadLevel();

                PrevPlayerMapIconPosition = playerIcon.Position;
            }
        }
    }

    private void CheckMove(string inputAction, Vector2 moveOffset)
    {
        if (Input.IsActionJustPressed(inputAction))
        {
            var currPos = playerIcon.Position;
            var nextPos = currPos + moveOffset;

            if (tileMapTerrain.GetTileName(nextPos) != "path")
                return;

            //var tileIdLevelOffset = GetCurrentTileId(tileMapLevelIcons, nextPos);
            //var tileIdLevelCurr = GetCurrentTileId(tileMapLevelIcons, currPos);

            // yellow circle = 0
            // gray circle   = 1

            var nextTilePos = tileMapLevelIcons.WorldToMap(nextPos);

            //if (levelManager.LevelPositions.ContainsKey(nextTilePos) && levelManager.LevelPositions[nextTilePos].Locked)
            //    return;

            playerIcon.Position = nextPos;
        }
    }
    private void _on_Player_Area_area_entered(Area2D area)
    {
        GameManager.Level.CurrentLevel = area.Name;
    }
}
