namespace Sankari;

public class TransitionManager : Node
{
    private Tween tween;

    public override void _Ready()
    {
        tween = new Tween();
        AddChild(tween);
    }

    public async Task AlphaToBlack(float duration = 1.5f)
    {
        tween.InterpolateProperty(this, "color", new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), duration);
        tween.Start();
        await Task.Delay((int)duration * 1000);
    }

    public void BlackToAlpha(float duration = 1.5f)
    {
        tween.InterpolateProperty(this, "color", new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), duration);
        tween.Start();
    }

    public async Task AlphaToBlackAndBack(float duration = 1f, float deadTime = 0.15f) 
    {
        await AlphaToBlack(duration);
        await Task.Delay((int)deadTime * 1000);
        BlackToAlpha(duration);
    }
}
