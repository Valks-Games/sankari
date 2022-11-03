namespace Sankari;

public class MovingEntityCommandGroundJump : EntityCommand<MovingEntity>
{
	public int JumpForce { get; set; } = 600; // Force applies when jumping
	public int MaxJumps { get; set; } = 1; // Max number of Jumps
	public bool AllowAirJumps { get; set; } = false; // Allow mid air jumping

	private int JumpCount { get; set; }

	public MovingEntityCommandGroundJump(MovingEntity entity) : base(entity) { }

	public override void Update(float delta)
	{
		// Check if Entity in on ground and not current moving away from it
		if (Entity.IsNearGround() && Entity.Velocity.y >=0)
		{
			JumpCount = 0;
		}
	}

	public override void Start()
	{
		if (JumpCount < MaxJumps && (Entity.IsNearGround() || AllowAirJumps))
		{
			GameManager.EventsPlayer.Notify(EventPlayer.OnJump);

			JumpCount++;
			//Velocity = new Vector2(Velocity.x, 0); // reset velocity before jump (is this really needed?)
			Entity.Velocity = Entity.Velocity - new Vector2(0, JumpForce);
		}
	}
}
