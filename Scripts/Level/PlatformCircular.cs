namespace Sankari;

public class PlatformCircular : APlatform
{
    [Export] public float Radius = 10;
    [Export] public float Speed = 2;

    private Vector2 startPos;
    private float angle;

    public override void _Ready()
    {
        Init();

        startPos = Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        angle += delta * Speed;
        Position = startPos + new Vector2(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius);
    }
}
