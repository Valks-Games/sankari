namespace Sankari;

public partial class Spike : Node
{
	private void _on_area_2d_area_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
			player.RemoveHealth(1);
	}
}
