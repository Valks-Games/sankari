namespace Sankari;

public abstract partial class Entity : StaticBody2D
{
    public float Delta { get; set; }
    public AnimatedSprite2D AnimatedSprite { get; set; }

    public sealed override void _Ready()
    {
        AnimatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        Init();
    }

    public sealed override void _PhysicsProcess(double delta)
    {
        Delta = (float)delta;
        UpdatePhysics();
    }

    public abstract void Init();

    public abstract void UpdatePhysics();
}
