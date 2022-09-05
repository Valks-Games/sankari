namespace Sankari;

public class Prefabs 
{
    public static PackedScene Map = LoadPrefab("Map.tscn");
    public static PackedScene CannonBall = LoadPrefab("Enemies/CannonBall.tscn");
    public static PackedScene PlayerDashTrace = LoadPrefab("PlayerDashTrace.tscn");
    public static PackedScene BasicEnemy = LoadPrefab("Enemies/BasicEnemy.tscn");

    private static PackedScene LoadPrefab(string path) => ResourceLoader.Load<PackedScene>($"res://Scenes/Prefabs/{path}");
}