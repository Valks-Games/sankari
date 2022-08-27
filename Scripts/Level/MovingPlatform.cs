namespace Sankari;

public class MovingPlatform : KinematicBody2D
{
    [Export] public int Duration = 3;
    [Export] public int StartDelay = 0;
    [Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Cubic;
    [Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;

    private GTimer _timer;
    private CollisionShape2D _collision;

    public override async void _Ready()
    {
        _collision = GetNode<CollisionShape2D>("CollisionShape2D");
        _timer = new GTimer(this, nameof(OnTimerUp), 400, false, false);

        var tween = new GTween(this);

        await tween.AnimatePlatform
        (
            Position, 
            GetNode<Position2D>("Target").GlobalPosition, 
            GetNode<Sprite>("Sprite").GetRect().Size.x,
            Duration, 
            StartDelay, 
            TransitionType, 
            EaseType
        );
    }

    public void TemporarilyDisablePlatform()
    {
        _collision.Disabled = true;
        _timer.Start();
    }

    private void OnTimerUp() 
    {
        _collision.Disabled = false;
    }
}
