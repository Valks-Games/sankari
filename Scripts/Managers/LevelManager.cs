namespace MarioLikeGame;

public class LevelManager
{
    public Dictionary<Vector2, Level> LevelPositions = new();
    public Dictionary<string, Level> LevelNames = new();

    public Dictionary<string, PackedScene> Levels = new();

    public string CurrentLevel { get; set; }

    private Node _nodeLevel;
    private GameManager _gameManager;

    public LevelManager(GameManager gameManager, Node nodeLevel)
    {
        _gameManager = gameManager;
        _nodeLevel = nodeLevel;
        var godotFileManager = new GodotFileManager();
        godotFileManager.LoadDir("Scenes/Levels", (dir, fileName) =>
        {
            if (!dir.CurrentIsDir())
                Levels[fileName.Replace(".tscn", "")] = ResourceLoader.Load<PackedScene>($"res://Scenes/Levels/{fileName}");
        });
    }

    public void LoadLevel()
    {
        // remove map
        _gameManager.DestroyMap();

        // load level
        var scenePath = $"res://Scenes/Levels/{CurrentLevel}.tscn";
        if (!new File().FileExists(scenePath))
        {
            GD.Print("Level has not been made yet!");
            return;
        }

        var levelPacked = ResourceLoader.Load<PackedScene>(scenePath);
        var level = (LevelScene)levelPacked.Instance();
        level.PreInit(_gameManager);
        _nodeLevel.AddChild(level);
    }

    public void CompleteLevel(string levelName)
    {
        // remove level
        _nodeLevel.QueueFreeChildren();

        // mark level as completed
        LevelNames[levelName].Completed = true;

        foreach (var level in LevelNames[levelName].Unlocks)
            if (LevelNames.ContainsKey(level))
                LevelNames[level].Locked = false;

        // load map
        _gameManager.LoadMap();
    }
}