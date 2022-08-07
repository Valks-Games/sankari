namespace MarioLikeGame;

public class Map : Node
{
    public static bool HasMapLoadedBefore { get; private set; }
    private static Vector2 _prevPlayerMapIconPosition { get; set; }

    [Export] protected readonly NodePath NodePathLevels;
    [Export] protected readonly NodePath NodePathTileMapLevelIcons;
    [Export] protected readonly NodePath NodePathTileMapTerrain;
    [Export] protected readonly NodePath NodePathPlayerIcon;

    private TileMap _tileMapLevelIcons, _tileMapTerrain;
    private Node _levels;
    private Sprite _playerIcon;

    private GameManager _gameManager;
    private LevelManager _levelManager;

    public void PreInit(GameManager gameManager) 
    {
        _gameManager = gameManager;
        _levelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        _tileMapLevelIcons = GetNode<TileMap>(NodePathTileMapLevelIcons);
        _tileMapTerrain = GetNode<TileMap>(NodePathTileMapTerrain);
        _levels = GetNode<Node>(NodePathLevels);
        _playerIcon = GetNode<Sprite>(NodePathPlayerIcon);

        foreach (var pos in _levelManager.LevelPositions.Keys) 
        {
            if (_levelManager.LevelPositions[pos].Completed)
                _tileMapLevelIcons.SetCellv(pos, 1); // remember 1 is gray circle
        }

        if (HasMapLoadedBefore) 
        {
            _playerIcon.Position = _prevPlayerMapIconPosition;
            return;
        }

        HasMapLoadedBefore = true;

        foreach (Area2D node in _levels.GetChildren())
        {
            var worldPos = ((CollisionShape2D)node.GetChild(0)).Position;
            var tilePos = _tileMapLevelIcons.WorldToMap(worldPos);

            _levelManager.LevelPositions.Add(tilePos, new Level(node.Name)
            {
                Locked = true
            });
        }

        foreach (var level in _levelManager.LevelPositions)
        {
            _levelManager.LevelNames.Add(level.Value.Name, level.Value);
        }

        _levelManager.LevelNames["Level A1"].Locked = false;
        _levelManager.LevelNames["Level B1"].Locked = false;
        _levelManager.LevelNames["Level C1"].Locked = false;
    }

    public override void _Input(InputEvent @event)
    {
        CheckMove("map_move_left", new Vector2(-16, 0));
        CheckMove("map_move_right", new Vector2(16, 0));
        CheckMove("map_move_up", new Vector2(0, -16));
        CheckMove("map_move_down", new Vector2(0, 16));

        if (Input.IsActionJustPressed("map_action"))
        {
            var id = GetCurrentTileId(_tileMapLevelIcons, _playerIcon.Position);
            if (id != -1)
            {
                _levelManager.LoadLevel();
                
                _prevPlayerMapIconPosition = _playerIcon.Position;

                // hide map and load level
                
            }
        }
    }

    private void CheckMove(string inputAction, Vector2 moveOffset)
    {
        if (Input.IsActionJustPressed(inputAction))
        {
            var currPos = _playerIcon.Position;
            var nextPos = currPos + moveOffset;

            var tileIdTerrain = GetCurrentTileId(_tileMapTerrain, nextPos);

            if (tileIdTerrain == 0) // this is not a path tile, cancel movement
                return;

            var tileIdLevelOffset = GetCurrentTileId(_tileMapLevelIcons, nextPos);
            var tileIdLevelCurr = GetCurrentTileId(_tileMapLevelIcons, currPos);

            // yellow circle = 0
            // gray circle   = 1

            var nextTilePos = _tileMapLevelIcons.WorldToMap(nextPos);

            if (_levelManager.LevelPositions.ContainsKey(nextTilePos) && _levelManager.LevelPositions[nextTilePos].Locked)
                return;

            _playerIcon.Position = nextPos;
        }
    }

    private int GetCurrentTileId(TileMap tilemap, Vector2 pos)
    {
        var cellPos = tilemap.WorldToMap(pos);
        return tilemap.GetCellv(cellPos);
    }

    private void _on_Player_Area_area_entered(Area2D area)
    {
        _levelManager.CurrentLevel = area.Name;
    }
}

public class Level 
{
    public string Name { get; set; }
    public bool Locked { get; set; }
    public bool Completed { get; set; }
    public List<string> Unlocks { get; set; }

    public Level(string name)
    {
        Name = name;
        Unlocks = new();

        var levelId = name.Split(" ")[1];
        var letter = levelId.Substring(0, 1);
        var num = int.Parse(levelId.Substring(1));
        
        num += 1;

        Unlocks.Add($"Level {letter}{num}"); // if this is Level A1, then this adds a unlock for Level A2
    }
}
