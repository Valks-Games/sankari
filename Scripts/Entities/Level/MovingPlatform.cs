namespace Sankari;

public partial class MovingPlatform : APlatform
{
    [Export] public int Duration { get; set; } = 3;
    [Export] public int StartDelay { get; set; } = 0;
    [Export] public Tween.TransitionType TransitionType { get; set; } = Tween.TransitionType.Cubic;
    [Export] public Tween.EaseType EaseType { get; set; } = Tween.EaseType.InOut;

    public override async void _Ready()
    {
        Init();

        var tween = new GTween(this);

        await tween.AnimatePlatform
        (
            Position, 
            GetNode<Marker2D>("Target").GlobalPosition, 
            GetNode<Sprite2D>("Sprite2D").GetRect().Size.X,
            Duration, 
            StartDelay, 
            TransitionType, 
            EaseType
        );
    }
}
