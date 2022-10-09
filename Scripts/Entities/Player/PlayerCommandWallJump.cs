namespace Sankari;

public class PlayerCommandWallJump : PlayerCommand
{
	public PlayerCommandWallJump(Player player) : base(player) { }

	public override void Start()
	{
		// on a wall and falling
		if (Player.InWallJumpArea)
		{
			Player.AnimatedSprite.FlipH = Player.WallDir == 1;

			if (Player.WallDir != 0 && Player.IsFalling())
			{
				var JumpForceWallHorz = 700;
				var JumpForceWallVert = 600;

				// wall jump
				Player.PlayerVelocity.x += -JumpForceWallHorz * Player.WallDir;
				Player.PlayerVelocity.y -= JumpForceWallVert;
			}
		}
		else
			Player.AnimatedSprite.FlipH = false;
	}

	public override void Update(float delta)
	{
		Player.WallDir = UpdateWallDirection();

		if (Player.WallDir != 0 && Player.InWallJumpArea)
		{
			if (Player.IsFalling())
			{
				Player.PlayerVelocity.y = 1;

				// fast fall
				if (Player.PlayerInput.IsDown)
					Player.PlayerVelocity.y += 200;
			}
		}
	}

	private int UpdateWallDirection()
	{
		var left = CollectionExtensions.IsAnyRayCastColliding(Player.RayCast2DWallChecksLeft);
		var right = CollectionExtensions.IsAnyRayCastColliding(Player.RayCast2DWallChecksRight);

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}
}
