namespace Sankari;

public class PlatformCircular : APlatform
{
    [Export] public float Radius = 10;
    [Export] public float Speed = 2;

    private Vector2 _startPos;
    private float _angle;

    public override void _Ready()
    {
        Init();

        _startPos = Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        _angle += delta * Speed;
        Position = _startPos + new Vector2(Mathf.Cos(_angle) * Radius, Mathf.Sin(_angle) * Radius);
    }
}
