namespace Sankari;

public partial class Bottom : Node
{
    private void _on_Bottom_area_entered(Area2D area) 
    {
        var parent = area.GetParent();

        if (parent is IEnemy) 
        {
            parent.QueueFree();
        }
    }
}
