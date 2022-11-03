namespace Sankari;

public abstract partial class Entity : StaticBody2D
{
	public float Delta { get; set; }

	public sealed override void _Ready()
	{
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
