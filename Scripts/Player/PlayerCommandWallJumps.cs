namespace Sankari;

public class PlayerCommandWallJumps : PlayerCommand
{
	public override void Update(Player player) 
	{
		player.WallDir = UpdateWallDirection(player);

		// on a wall and falling
		if (player.WallDir != 0 && player.InWallJumpArea)
		{
			player.AnimatedSprite.FlipH = player.WallDir == 1;

			if (player.IsFalling())
			{
				player.velocityPlayer.y = 0;

				// fast fall
				if (player.InputDown)
					player.velocityPlayer.y += 50;

				// wall jump
				if (player.InputJump)
				{
					player.Jump();
					player.velocityPlayer.x += -player.JumpForceWallHorz * player.WallDir;
					player.velocityPlayer.y -= player.JumpForceWallVert;
				}
			}
		}
		else
		{
			player.AnimatedSprite.FlipH = false;
		}
	}

	private int UpdateWallDirection(Player player)
	{
		var left = IsTouchingWallLeft(player);
		var right = IsTouchingWallRight(player);

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}

	private bool IsTouchingWallLeft(Player player)
	{
		foreach (var raycast in player.RayCast2DWallChecksLeft)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	private bool IsTouchingWallRight(Player player)
	{
		foreach (var raycast in player.RayCast2DWallChecksRight)
			if (raycast.IsColliding())
				return true;

		return false;
	}
}
