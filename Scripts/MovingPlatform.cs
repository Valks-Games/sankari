namespace Sankari;

public class MovingPlatform : KinematicBody2D
{
    [Export] public int Duration = 3;
    [Export] public int StartDelay = 0;
    [Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Cubic;
    [Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;

    public override async void _Ready()
    {
        var tween = new GTween(this);

        await tween.AnimatePlatform
        (
            Position, 
            GetNode<Position2D>("Target").GlobalPosition, 
            Duration, 
            StartDelay, 
            TransitionType, 
            EaseType
        );
    }
}
