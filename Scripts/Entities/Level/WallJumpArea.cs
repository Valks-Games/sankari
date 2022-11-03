namespace Sankari;

public partial class WallJumpArea : Area2D
{
	private void _on_area_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.InWallJumpArea = true;
		}
	}

	private void _on_area_exited(Area2D area)
	{
		if (area.GetParent() is Player player)
		{
			player.InWallJumpArea = false;
		}
	}
}
