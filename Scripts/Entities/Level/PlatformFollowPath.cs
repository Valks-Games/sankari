namespace Sankari;

public partial class PlatformFollowPath : APlatform
{
    //[Export] public float Speed = 10f;

    private PathFollow2D Path { get; set; }
    private CollisionShape2D Collider { get; set; }

    public override void _Ready()
    {
        Init();

        Path = GetNode<PathFollow2D>("Path2D/PathFollow2D");
        Collider = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Path.Progress += (float)delta * 20;
        Collider.Position = Path.Position;
    }
}
