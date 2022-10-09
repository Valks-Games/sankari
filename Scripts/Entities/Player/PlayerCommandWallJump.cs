namespace Sankari;

public class PlayerCommandWallJump : PlayerCommand
{
	public PlayerCommandWallJump(Player player) : base(player) { }

	public override void Update(float delta)
	{
		Player.WallDir = UpdateWallDirection();

		// on a wall and falling
		if (Player.WallDir != 0 && Player.InWallJumpArea)
		{
			Player.AnimatedSprite.FlipH = Player.WallDir == 1;

			if (Player.IsFalling())
			{
				Player.PlayerVelocity.y = 1;

				// fast fall
				if (Player.PlayerInput.IsDown)
					Player.PlayerVelocity.y += 200;

				var JumpForceWallHorz = 500;
				var JumpForceWallVert = 500;

				// wall jump
				if (Player.PlayerInput.IsJump)
				{
					Player.PlayerVelocity.x += -JumpForceWallHorz * Player.WallDir;
					Player.PlayerVelocity.y -= JumpForceWallVert;
				}
			}
		}
		else
			Player.AnimatedSprite.FlipH = false;
	}

	private int UpdateWallDirection()
	{
		var left = CollectionExtensions.IsAnyRayCastColliding(Player.RayCast2DWallChecksLeft);
		var right = CollectionExtensions.IsAnyRayCastColliding(Player.RayCast2DWallChecksRight);

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}
}
