namespace Sankari;

public class PlatformFollowPath : APlatform
{
    //[Export] public float Speed = 10f;

    private PathFollow2D _path;
    private CollisionShape2D _collision;

    public override void _Ready()
    {
        Init();

        _path = GetNode<PathFollow2D>("Path2D/PathFollow2D");
        _collision = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _PhysicsProcess(float delta)
    {
        _path.Offset += delta * 20;
        _collision.Position = _path.Position;
    }
}
