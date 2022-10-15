namespace Sankari;

public static class Prefabs
{
    public static PackedScene Map { get; }             = LoadPrefab("Map");
    public static PackedScene PlayerDashTrace { get; } = LoadPrefab("PlayerDashTrace");
    public static PackedScene BasicEnemy { get; }      = LoadPrefab("Enemies/BasicEnemy");
    public static PackedScene PopupMessage { get; }    = LoadPrefab("UI/Popups/PopupMessage");
    public static PackedScene PopupError { get; }      = LoadPrefab("UI/Popups/PopupError");
    public static PackedScene PopupLineEdit { get; }   = LoadPrefab("UI/Popups/PopupLineEdit");
    public static PackedScene OtherPlayer { get; }     = LoadPrefab("OtherPlayer");

    private static PackedScene LoadPrefab(string path) => ResourceLoader.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}
