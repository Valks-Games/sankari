namespace MarioLikeGame;

public class PlayerDashTrace : Sprite
{
    private GTimer _timer;

    public override void _Ready()
    {
        _timer = new GTimer(this, nameof(OnTimerDone), 200, false, true);
    }

    private void OnTimerDone()
    {
        QueueFree();
    }
}
