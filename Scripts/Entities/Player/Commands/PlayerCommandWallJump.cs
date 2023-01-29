namespace Sankari;

public class PlayerCommandWallJump : PlayerCommand<Player>
{
	// constants
	public int JumpForceWallHorz { get; set; } = 800; // Horizontal wall jump force
	public int JumpForceWallVert { get; set; } = 500; // Vertical wall jump force
	public int MaxGravitySpeedSliding { get; set; } = 20; // Sliding force downwards
	public int MaxGravitySpeedSlidingFast { get; set; } = 220; // Fast sliding force downwards
	private float JumpForceMultiplierVert { get; set; } = 0.95f;
	private float JumpForceMultiplierHorz { get; set; } = 0.5f;
	private int JumpsOnSameWallMax { get; set; } = 2;

	// variables
	private int PreviousWall { get; set; }
	private int WallDir { get; set; }
	private int JumpsOnSameWall { get; set; }

	// events
	public event EventHandler WallJump;

	public PlayerCommandWallJump(Player player) : base(player) { }

	public override void Start()
	{
		if (!Entity.InWallJumpArea)
		{
			Entity.AnimatedSprite.FlipH = false;
			return;
		}

		if (!OnAWall())
			return;

		Entity.AnimatedSprite.FlipH = WallDir == 1; // flip sprite on wall jump

		var velocity = Entity.Velocity;

		if (JumpOnSameWall())
		{
			// Only jump on the same wall if wall jumps does not exceed maximum same wall jumps
			if (++JumpsOnSameWall >= JumpsOnSameWallMax)
				return;

			WallJump?.Invoke(this, EventArgs.Empty);

			// Decrease value to reduce the jump force multiplier further
			// A value of 0.1 would be very dramatic while a value of 0.95
			// would be hard to notice
			var jumpForceReductionVert = 0.75f;
			var jumpForceReductionHorz = 0.5f;

			// Reduce wall jump force multiplier if jumping from the same wall
			// For e.g. a jump force multiplier of 0.75 would become 0.60
			// then 0.48 and so on
			// If the player jumped on the same wall 5 times they would gain
			// no more jump force
			JumpForceMultiplierVert *= jumpForceReductionVert;
			JumpForceMultiplierHorz *= jumpForceReductionHorz;

			if (EntityIsFacingWall())
			{
				velocity.X += -JumpForceWallHorz * WallDir * JumpForceMultiplierHorz * 0.3f;
			}
			else
				velocity.X += -JumpForceWallHorz * WallDir * JumpForceMultiplierHorz;

			velocity.Y = -JumpForceWallVert * JumpForceMultiplierVert;
		}
		else
		{
			// Wall jump is on a different wall, reset jumps on same wall
			JumpsOnSameWall = 0;

			WallJump?.Invoke(this, EventArgs.Empty);

			// Wall jump is on a diferent wall, reset the jump force multipliers to
			// their default values
			JumpForceMultiplierVert = 0.95f;
			JumpForceMultiplierHorz = 0.5f;

			if (EntityIsFacingWall())
			{
				velocity.X += -JumpForceWallHorz * WallDir * 0.3f;
			}
			else
				velocity.X += -JumpForceWallHorz * WallDir;

			velocity.Y = -JumpForceWallVert;
		}

		Entity.Velocity = velocity;

		PreviousWall = WallDir;
	}

	public override void Update(float delta)
	{
		WallDir = UpdateWallDirection();

		if (Entity.IsNearGround())
		{
			PreviousWall = 0;
			return;
		}

		if (Entity.InWallJumpArea && OnAWall())
		{
			var velocity = Entity.Velocity;

			if (Entity.IsFalling())
			{
				Entity.FakeGravitySpeed = MaxGravitySpeedSliding;

				// fast fall
				if (Entity is Player player)
					if (player.PlayerInput.IsDown)
						Entity.FakeGravitySpeed = MaxGravitySpeedSlidingFast;

				// Slow down the player
				//if (velocity.Y != Entity.FakeGravitySpeed)
					velocity = velocity.MoveToward(new Vector2(velocity.X, Entity.FakeGravitySpeed), 2000 * delta);
			}

			Entity.Velocity = velocity;
		}
	}

	private bool EntityIsFacingWall() => Entity.MoveDir.X == WallDir;
	private bool OnAWall() => WallDir != 0;
	private bool JumpOnSameWall() => PreviousWall == WallDir;

	private int UpdateWallDirection()
	{
		var left = Entity.RaycastsWallLeft.IsAnyRayCastColliding();
		var right = Entity.RaycastsWallRight.IsAnyRayCastColliding();

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}
}
