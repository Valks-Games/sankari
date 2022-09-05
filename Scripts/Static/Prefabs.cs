namespace Sankari;

public class Prefabs 
{
    public readonly static PackedScene Map = LoadPrefab("Map");
    public readonly static PackedScene CannonBall = LoadPrefab("Enemies/CannonBall");
    public readonly static PackedScene PlayerDashTrace = LoadPrefab("PlayerDashTrace");
    public readonly static PackedScene BasicEnemy = LoadPrefab("Enemies/BasicEnemy");
    public readonly static PackedScene PopupMessage = LoadPrefab("UI/Popups/PopupMessage");
    public readonly static PackedScene PopupError = LoadPrefab("UI/Popups/PopupError");
    public readonly static PackedScene PopupLineEdit = LoadPrefab("UI/Popups/PopupLineEdit");

    private static PackedScene LoadPrefab(string path) => ResourceLoader.Load<PackedScene>($"res://Scenes/Prefabs/{path}.tscn");
}