namespace Sankari;

public interface IEntityJumpable : IEntityMovement
{
}

public interface IEntityGroundJumpable : IEntityJumpable
{
	bool IsFalling();
}

public class EntityCommandGroundJump : EntityCommand<IEntityGroundJumpable>
{
	#region Configuration

	// Force applies when jumping
	public int JumpForce { get; set; } = 600;

	// Max number of Jumps
	public int MaxJumps { get; set; } = 1;

	// Allow mid air jumping
	public bool AllowAirJumps { get; set; } = false;

	#endregion

	private int JumpCount { get; set; }

	public EntityCommandGroundJump(IEntityGroundJumpable entity) : base(entity) { }

	public override void Update(float delta)
	{
		// Check if Entity in on ground and not current moving away from it
		if (Entity.IsOnGround() && Entity.Velocity.y >=0)
		{
			JumpCount = 0;
		}
	}

	public override void Start()
	{
		if (JumpCount < MaxJumps && (Entity.IsOnGround() || AllowAirJumps))
		{
			GameManager.EventsPlayer.Notify(EventPlayer.OnJump);

			JumpCount++;
			//Velocity = new Vector2(Velocity.x, 0); // reset velocity before jump (is this really needed?)
			Entity.Velocity = Entity.Velocity - new Vector2(0, JumpForce);
		}
	}
}
