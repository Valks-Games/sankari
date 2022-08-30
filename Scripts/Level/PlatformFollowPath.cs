namespace Sankari;

public class PlatformFollowPath : APlatform
{
    //[Export] public float Speed = 10f;

    private PathFollow2D path;
    private CollisionShape2D collision;

    public override void _Ready()
    {
        Init();

        path = GetNode<PathFollow2D>("Path2D/PathFollow2D");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _PhysicsProcess(float delta)
    {
        path.Offset += delta * 20;
        collision.Position = path.Position;
    }
}
