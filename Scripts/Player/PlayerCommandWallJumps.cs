namespace Sankari;

public class PlayerCommandWallJumps : PlayerCommand
{
	public PlayerCommandWallJumps(IMoveableEntity entity) : base(entity)
	{
	}

	public override void Update(MovementInput input)
	{
		Vector2 velocity = Entity.Velocity;
		// on a wall and falling
		if (Entity.WallDir != 0 && Entity.InWallJumpArea)
		{
			Entity.AnimatedSprite.FlipH = Entity.WallDir == 1;

			if (Entity.IsFalling())
			{
				velocity.y = 0;

				// fast fall
				if (input.IsDown)
					velocity.y += 50;

				// wall jump
				if (input.IsJump)
				{
					Entity.Jump();
					velocity.x += -Entity.JumpForceWallHorz * Entity.WallDir;
					velocity.y -= Entity.JumpForceWallVert;
				}
			}
		}
		else
		{
			Entity.AnimatedSprite.FlipH = false;
		}

		Entity.Velocity = velocity;
	}
}
