namespace Sankari;

public class PlatformCircular : APlatform
{
    [Export] public float Speed = 2;

    private Vector2 startPos;
    private float angle;
    private float radius;
    private CollisionShape2D collisionShapeRadius;

    public override void _Ready()
    {
        Init();
        collisionShapeRadius = GetNode<CollisionShape2D>("Radius");

        var spriteWidth = GetNode<Sprite>("Sprite").Texture.GetSize().x;

        radius = (collisionShapeRadius.Shape as CircleShape2D).Radius - (spriteWidth / 2);
        startPos = Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        angle += delta * Speed;
        Position = startPos + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }
}
