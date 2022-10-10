namespace Sankari;

public class PlayerCommandWallJump : PlayerCommand
{
	public PlayerCommandWallJump(Player player) : base(player) { }

	public override void Start()
	{
		// on a wall and falling
		if (Entity.InWallJumpArea)
		{
			Entity.AnimatedSprite.FlipH = Entity.WallDir == 1;

			if (Entity.WallDir != 0 && Entity.IsFalling())
			{
				var JumpForceWallHorz = 700;
				var JumpForceWallVert = 600;

				// wall jump
				var velocity = Entity.Velocity;
				velocity.x += -JumpForceWallHorz * Entity.WallDir;
				velocity.y -= JumpForceWallVert;
				Entity.Velocity = velocity;
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
				if (Entity.PlayerInput.IsDown)
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
