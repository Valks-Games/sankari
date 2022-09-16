namespace Sankari;

public abstract partial class APlatform : CharacterBody2D
{
	protected CollisionShape2D Collision { get; private set; }
	
	private GTimer timer;

	public void Init()
	{
		timer = new GTimer(this, nameof(OnTimerUp), 400, false, false);
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public void TemporarilyDisablePlatform() 
	{
		timer.Start();
		Collision.Disabled = true;
	}

	private void OnTimerUp()
	{
		Collision.Disabled = false;
	}
}
