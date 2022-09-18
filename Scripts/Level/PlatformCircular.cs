namespace Sankari;

public partial class PlatformCircular : APlatform
{
    [Export] public float Speed { get; set; } = 2;

    private Vector2 StartPos { get; set; }
    private float Angle { get; set; }
    private float Radius { get; set; }
    private CollisionShape2D CollisionShapeRadius { get; set; }

    public override void _Ready()
    {
        Init();
        CollisionShapeRadius = GetNode<CollisionShape2D>("Radius");

        var spriteWidth = GetNode<Sprite2D>("Sprite2D").Texture.GetSize().x;

        Radius = (CollisionShapeRadius.Shape as CircleShape2D).Radius - (spriteWidth / 2);
        StartPos = Position;
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        Angle += delta * Speed;
        Position = StartPos + new Vector2(Mathf.Cos(Angle) * Radius, Mathf.Sin(Angle) * Radius);
    }
}
