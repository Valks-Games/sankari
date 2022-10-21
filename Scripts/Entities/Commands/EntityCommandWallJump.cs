namespace Sankari;

public interface IEntityWallJumpable : IEntityMoveable
{
	// Left wall checks
	public List<RayCast2D> RayCast2DWallChecksLeft  { get; }

	// Right wall checks
	public List<RayCast2D> RayCast2DWallChecksRight { get; }

	// Is entity within wall jump-able area
	public bool InWallJumpArea { get; }

	// Wall direction
	public int WallDir { get; set; }

	// Is the entity falling?
	public bool IsFalling();

	// Horizontal wall jump force
	public int JumpForceWallHorz { get; set; }

	// Vertical wall jump force
	public int JumpForceWallVert { get; set; }

	public int PreviousWallOnJump { get; set; }
}

public class EntityCommandWallJump : EntityCommand<IEntityWallJumpable>
{
	public EntityCommandWallJump(IEntityWallJumpable entity) : base(entity) { }

	public override void Start()
	{
		// on a wall and falling
		if (Entity.InWallJumpArea)
		{
			Entity.AnimatedSprite.FlipH = Entity.WallDir == 1;

			// If the entity is on a wall, prevent entity from wall jumping on the same wall twice
			if (Entity.WallDir != 0 && Entity.PreviousWallOnJump != Entity.WallDir)
			{
				// wall jump
				GameManager.EventsPlayer.Notify(EventPlayer.OnJump);

				var velocity = Entity.Velocity;
				velocity.x += -Entity.JumpForceWallHorz * Entity.WallDir;
				velocity.y -= Entity.JumpForceWallVert;
				Entity.Velocity = velocity;

				Entity.PreviousWallOnJump = Entity.WallDir;
			}
		}
		else
			Entity.AnimatedSprite.FlipH = false;
	}

	public override void Update(float delta)
	{
		Entity.WallDir = UpdateWallDirection();

		if (Entity.WallDir != 0 && Entity.InWallJumpArea)
		{
			if (Entity.IsFalling())
			{
				var velocity = Entity.Velocity;
				velocity.y = 1;

				// fast fall
				if (Entity is Player player)
					if (player.PlayerInput.IsDown)
						velocity.y += 200;

				Entity.Velocity = velocity;
			}
		}
	}

	private int UpdateWallDirection()
	{
		var left = CollectionExtensions.IsAnyRayCastColliding(Entity.RayCast2DWallChecksLeft);
		var right = CollectionExtensions.IsAnyRayCastColliding(Entity.RayCast2DWallChecksRight);

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}
}
