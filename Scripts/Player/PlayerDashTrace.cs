namespace Sankari;

public class PlayerDashTrace : Sprite
{
    private GTimer timer;

    public override void _Ready()
    {
        timer = new GTimer(this, nameof(OnTimerDone), 200, false, true);
    }

    private void OnTimerDone()
    {
        QueueFree();
    }
}
