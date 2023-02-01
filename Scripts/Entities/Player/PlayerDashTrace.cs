namespace Sankari;

public partial class PlayerDashTrace : Sprite2D
{
    public override void _Ready() =>
		GetTree().CreateTimer(0.1).Timeout += () => QueueFree();
}
