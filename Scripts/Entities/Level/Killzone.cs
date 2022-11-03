namespace Sankari;

public partial class Killzone : Area2D 
{
    private void _on_Bottom_area_entered(Area2D area) 
    {
        var parent = area.GetParent();

		if (parent is Player player)
		{
			player.Kill();
			return; // do not queue free the player by accident
		} 

        if (parent is MovingEntity)
		{
            parent.QueueFree();
			return; // do not do something else by accident
		}
    }
}
