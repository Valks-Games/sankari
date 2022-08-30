namespace Sankari;

public class Map : Node
{
    public static bool HasMapLoadedBefore { get; private set; }
    private static Vector2 prevPlayerMapIconPosition { get; set; }

    [Export] protected readonly NodePath NodePathLevels;
    [Export] protected readonly NodePath NodePathTileMapLevelIcons;
    [Export] protected readonly NodePath NodePathTileMapTerrain;
    [Export] protected readonly NodePath NodePathPlayerIcon;

    private TileMap tileMapLevelIcons, tileMapTerrain;
    private Node levels;
    private Sprite playerIcon;

    private GameManager gameManager;
    private LevelManager levelManager;
    private bool loadingLevel;

    public void PreInit(GameManager gameManager) 
    {
        this.gameManager = gameManager;
        levelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        tileMapLevelIcons = GetNode<TileMap>(NodePathTileMapLevelIcons);
        tileMapTerrain = GetNode<TileMap>(NodePathTileMapTerrain);
        levels = GetNode<Node>(NodePathLevels);
        playerIcon = GetNode<Sprite>(NodePathPlayerIcon);

        foreach (var level in levelManager.Levels.Values) 
            if (level.Completed)
                tileMapLevelIcons.SetCellv(level.Position, 1); // remember 1 is gray circle

        if (HasMapLoadedBefore) 
        {
            playerIcon.Position = prevPlayerMapIconPosition;
            return;
        }

        HasMapLoadedBefore = true;

        foreach (Area2D levelArea in levels.GetChildren())
        {
            var worldPos = ((CollisionShape2D)levelArea.GetChild(0)).Position;
            var tilePos = tileMapLevelIcons.WorldToMap(worldPos);

            if (!levelManager.Levels.ContainsKey(levelArea.Name)) // level has not been defined in LevelManager.cs
                levelManager.Levels.Add(levelArea.Name, new Level(levelArea.Name) {
                    Position = tilePos
                });
            else
                levelManager.Levels[levelArea.Name].Position = tilePos;
        }
    }

    public override async void _Input(InputEvent @event)
    {
        CheckMove("map_move_left", new Vector2(-16, 0));
        CheckMove("map_move_right", new Vector2(16, 0));
        CheckMove("map_move_up", new Vector2(0, -16));
        CheckMove("map_move_down", new Vector2(0, 16));

        if (Input.IsActionJustPressed("map_action") && !loadingLevel)
        {
            var id = GetCurrentTileId(tileMapLevelIcons, playerIcon.Position);
            if (id != -1)
            {
                loadingLevel = true;
                await gameManager.TransitionManager.AlphaToBlackAndBack();

                levelManager.LoadLevel();
                
                prevPlayerMapIconPosition = playerIcon.Position;

                // hide map and load level
                
            }
        }
    }

    private void CheckMove(string inputAction, Vector2 moveOffset)
    {
        if (Input.IsActionJustPressed(inputAction))
        {
            var currPos = playerIcon.Position;
            var nextPos = currPos + moveOffset;

            var tileIdTerrain = GetCurrentTileId(tileMapTerrain, nextPos);

            if (tileIdTerrain == 0) // this is not a path tile, cancel movement
                return;

            var tileIdLevelOffset = GetCurrentTileId(tileMapLevelIcons, nextPos);
            var tileIdLevelCurr = GetCurrentTileId(tileMapLevelIcons, currPos);

            // yellow circle = 0
            // gray circle   = 1

            var nextTilePos = tileMapLevelIcons.WorldToMap(nextPos);

            //if (_levelManager.LevelPositions.ContainsKey(nextTilePos) && _levelManager.LevelPositions[nextTilePos].Locked)
             //   return;

            playerIcon.Position = nextPos;
        }
    }

    // This method is useless because it's returning values like "tiles_packed.png 1" and "tiles_packed.png 4"
    // This is suppose to be fixed in Godot 4
    private string GetTileName(TileMap tilemap, Vector2 pos) =>
        tilemap.TileSet.TileGetName(GetCurrentTileId(tilemap, pos));

    private int GetCurrentTileId(TileMap tilemap, Vector2 pos)
    {
        var cellPos = tilemap.WorldToMap(pos);
        return tilemap.GetCellv(cellPos);
    }

    private void _on_Player_Area_area_entered(Area2D area)
    {
        levelManager.CurrentLevel = area.Name;
    }
}
