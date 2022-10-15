namespace Sankari;

public static class LevelManager
{
    public static Dictionary<string, Level> Levels { get; set; } = new();

    public static Dictionary<string, PackedScene> Scenes { get; set; } = new();

    public static string CurrentLevel { get; set; }

    private static Node NodeLevel { get; set; }

    public static void Init(Node nodeLevel)
    {
        NodeLevel = nodeLevel;

		GodotFileManager.LoadDir("Scenes/Levels", (dir, fileName) =>
		{
			if (!dir.CurrentIsDir())
				Scenes[fileName.Replace(".tscn", "")] = ResourceLoader.Load<PackedScene>($"res://Scenes/Levels/{fileName}");
		});

		// test level
		AddLevel("Test Level");

        AddLevel("Level A1", "grassy_1", 0.9f, false);
        AddLevel("Level A2", "grassy_2", 0.9f);

        // unused
        AddLevel("Level B1", "", 1, false);

        // unused
        AddLevel("Level C1", "", 1, false);
    }

    private static void AddLevel(string name, string music = "", float musicPitch = 1, bool locked = true) =>
        Levels.Add(name, new Level(name) {
            Locked = locked,
            Music = music,
            MusicPitch = musicPitch
        });

    public static async Task LoadLevel(bool instant = false)
    {
		if (!instant)
			await GameManager.Transition.AlphaToBlackAndBack();

        LoadALevel();
    }

    public static void LoadLevelFast() => LoadALevel();

    private static void LoadALevel()
    {
        // remove map
        GameManager.DestroyMap();

        // remove level if any
        NodeLevel.QueueFreeChildren();

        // load level
        var scenePath = $"res://Scenes/Levels/{CurrentLevel}.tscn";
        if (!FileAccess.FileExists(scenePath))
        {
            Logger.LogWarning("Level has not been made yet!");
            return;
        }

        var levelPacked = ResourceLoader.Load<PackedScene>(scenePath);

		if (levelPacked.Instantiate() is LevelScene level)
		{
			level.PreInit();
			NodeLevel.AddChild(level);

			var curLevel = Levels[CurrentLevel];

			Audio.PlayMusic(curLevel.Music, curLevel.MusicPitch);
			GameManager.LevelUI.AddHealth(6);
			GameManager.LevelUI.Show();
		}
		else
		{
			Logger.LogWarning
			(
				"Level does not have LevelScene.cs script attached " +
				"to root node. Some things may not function as expected."
			);
		}
    }

    public static async Task CompleteLevel(string levelName)
    {
		// TODO: Clear player health at the end of each level or hide it so it is
		// not visible in the map or menu screens. It should only be visible when
		// in a level.

        // the level scene is no longer active, clean it up
        GameManager.Events.RemoveListener(nameof(LevelManager), Event.OnGameClientLeft);
        GameManager.LevelScene = null;

        await GameManager.Transition.AlphaToBlackAndBack();

        // remove level
        NodeLevel.QueueFreeChildren();

        // mark level as completed
        Levels[levelName].Completed = true;

        foreach (var level in Levels[levelName].Unlocks)
            if (Levels.ContainsKey(level))
                Levels[level].Locked = false;

        // load map
        GameManager.LoadMap();
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

		// for example "Test Level" does not have a number in the name
		if (!int.TryParse(levelId.Substring(1), out int num))
			return;
        
        num += 1;

        Unlocks.Add($"Level {letter}{num}"); // if this is Level A1, then this adds a unlock for Level A2
    }
}
