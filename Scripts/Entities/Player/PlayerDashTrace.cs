namespace Sankari;

public partial class PlayerDashTrace : Sprite2D
{
    private GTimer Timer { get; set; }

    public override void _Ready()
    {
        Timer = new GTimer(this, nameof(OnTimerDone), 200, false, true);
    }

    private void OnTimerDone()
    {
        QueueFree();
    }
}
