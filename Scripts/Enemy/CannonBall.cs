namespace MarioLikeGame;

public class CannonBall : RigidBody2D
{
    private GTimer _timer;

    public override void _Ready()
    {
        _timer = new GTimer(this, nameof(OnTimerEnd), 3000, false);
    }

    private void OnTimerEnd()
    {
        QueueFree();
    }
}