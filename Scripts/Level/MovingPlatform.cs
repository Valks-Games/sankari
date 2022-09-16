namespace Sankari;

public partial class MovingPlatform : APlatform
{
    [Export] public int Duration = 3;
    [Export] public int StartDelay = 0;
    [Export] public Tween.TransitionType TransitionType = Tween.TransitionType.Cubic;
    [Export] public Tween.EaseType EaseType = Tween.EaseType.InOut;

    public override async void _Ready()
    {
        Init();

        var tween = new GTween(this);

        await tween.AnimatePlatform
        (
            Position, 
            GetNode<Marker2D>("Target").GlobalPosition, 
            GetNode<Sprite2D>("Sprite2D").GetRect().Size.x,
            Duration, 
            StartDelay, 
            TransitionType, 
            EaseType
        );
    }
}
