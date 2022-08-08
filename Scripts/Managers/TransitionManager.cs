namespace MarioLikeGame;

public class TransitionManager : Node
{
    private Tween _tween;

    public override void _Ready()
    {
        _tween = new Tween();
        AddChild(_tween);
    }

    public void AlphaToBlack(float duration = 1.5f)
    {
        _tween.InterpolateProperty(this, "color", new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), duration);
        _tween.Start();
    }

    public void BlackToAlpha(float duration = 1.5f)
    {
        _tween.InterpolateProperty(this, "color", new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), duration);
        _tween.Start();
    }

    public async Task AlphaToBlackAndBack(float duration = 1f, float deadTime = 0.15f) 
    {
        AlphaToBlack(duration);
        _tween.Start();
        await Task.Delay((int)(duration * 1000) + (int)(deadTime * 1000));
        BlackToAlpha(duration);
        _tween.Start();
    }
}
