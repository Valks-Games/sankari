namespace Sankari;

public class LevelManager
{
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

        AddLevel("Level A1", "grassy_1", 0.9f, false);
        AddLevel("Level A2", "grassy_2", 0.9f);

        // unused
        AddLevel("Level B1", "", 1, false);

        // unused
        AddLevel("Level C1", "", 1, false);
    }

    private void AddLevel(string name, string music = "", float musicPitch = 1, bool locked = true) =>
        LevelNames.Add(name, new Level(name) {
            Locked = locked,
            Music = music,
            MusicPitch = musicPitch
        });

    public void LoadLevel()
    {
        // remove map
        _gameManager.DestroyMap();

        // remove level if any
        _nodeLevel.QueueFreeChildren();

        // load level
        var scenePath = $"res://Scenes/Levels/{CurrentLevel}.tscn";
        if (!new File().FileExists(scenePath))
        {
            Logger.LogWarning("Level has not been made yet!");
            return;
        }

        var levelPacked = ResourceLoader.Load<PackedScene>(scenePath);
        var level = (LevelScene)levelPacked.Instance();
        level.PreInit(_gameManager);
        _nodeLevel.AddChild(level);

        var curLevel = LevelNames[CurrentLevel];

        _gameManager.Audio.PlayMusic(curLevel.Music, curLevel.MusicPitch);
    }

    public async Task CompleteLevel(string levelName)
    {
        await _gameManager.TransitionManager.AlphaToBlackAndBack();

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

public class Level 
{
    public string Name { get; set; }
    public string Music { get; set; }
    public float MusicPitch { get; set; }
    public bool Locked { get; set; }
    public bool Completed { get; set; }
    public Vector2 Position { get; set; }
    public List<string> Unlocks { get; set; }

    public Level(string name)
    {
        Name = name;
        Unlocks = new();
        MusicPitch = 1.0f;

        var levelId = name.Split(" ")[1];
        var letter = levelId.Substring(0, 1);
        var num = int.Parse(levelId.Substring(1));
        
        num += 1;

        Unlocks.Add($"Level {letter}{num}"); // if this is Level A1, then this adds a unlock for Level A2
    }
}
