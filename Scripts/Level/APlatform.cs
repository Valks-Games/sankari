namespace Sankari;

public abstract class APlatform : KinematicBody2D
{
    private GTimer _timer;
    protected CollisionShape2D Collision { get; private set; }

    public void Init()
    {
        _timer = new GTimer(this, nameof(OnTimerUp), 400, false, false);
        Collision = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public void TemporarilyDisablePlatform() 
    {
        _timer.Start();
        Collision.Disabled = true;
    }

    private void OnTimerUp()
    {
        Collision.Disabled = false;
    }
}