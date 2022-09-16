namespace Sankari;

public partial class CannonBall : RigidBody2D
{
    private GTimer timer;

    public override void _Ready()
    {
        timer = new GTimer(this, nameof(OnTimerEnd), 3000, false);
    }

    private void OnTimerEnd()
    {
        QueueFree();
    }
}