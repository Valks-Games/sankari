namespace Sankari;

public static class Prefabs
{
    public static PackedScene Map             { get; } = LoadPrefab("Map");
    public static PackedScene PlayerDashTrace { get; } = LoadPrefab("PlayerDashTrace");
    public static PackedScene BasicEnemy      { get; } = LoadPrefab("Enemies/BasicEnemy");
    public static PackedScene PopupMessage    { get; } = LoadPrefab("UI/Popups/PopupMessage");
    public static PackedScene PopupError      { get; } = LoadPrefab("UI/Popups/PopupError");
    public static PackedScene PopupLineEdit   { get; } = LoadPrefab("UI/Popups/PopupLineEdit");
    public static PackedScene OtherPlayer     { get; } = LoadPrefab("OtherPlayer");

    private static PackedScene LoadPrefab(string path) => ResourceLoader.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}

public static class Textures 
{
	public static Texture2D FullHeart { get; } = LoadTexture("icon.png");
	public static Texture2D HalfHeart { get; } = LoadTexture("light.png");

	private static Texture2D LoadTexture(string path) => GD.Load<Texture2D>($"res://Sprites/{path}");
}
