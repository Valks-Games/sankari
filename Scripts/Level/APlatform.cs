namespace Sankari;

public abstract class APlatform : KinematicBody2D
{
    private GTimer _timer;
    private CollisionShape2D _collision;

    public void Init()
    {
        _timer = new GTimer(this, nameof(OnTimerUp), 400, false, false);
        _collision = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public void TemporarilyDisablePlatform() 
    {
        _timer.Start();
        _collision.Disabled = true;
    }

    private void OnTimerUp()
    {
        _collision.Disabled = false;
    }
}