namespace Sankari;

public partial class CannonBall : RigidBody2D
{
    private GTimer Timer { get; set; }

    public override void _Ready()
    {
        Timer = new GTimer(this, nameof(OnTimerEnd), 3000, false);
    }

    private void OnTimerEnd()
    {
        QueueFree();
    }
}