namespace Sankari;

public partial class TransitionManager : Node
{
    public async Task AlphaToBlack(float duration = 1.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "color", new Color(0, 0, 0, 1), duration);
        tween.Play();
        await Task.Delay((int)duration * 1000);
    }

    public void BlackToAlpha(float duration = 1.5f)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "color", new Color(0, 0, 0, 0), duration);
        tween.Play();
    }

    public async Task AlphaToBlackAndBack(float duration = 1f, float deadTime = 0.15f) 
    {
        await AlphaToBlack(duration);
        await Task.Delay((int)deadTime * 1000);
        BlackToAlpha(duration);
    }
}
