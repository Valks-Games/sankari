namespace Sankari;

public partial class GTween 
{
    private readonly Tween tween;
    private readonly Node target;

    public GTween(Node target)
    {
        this.target = target;
        tween = this.target.GetTree().CreateTween();
        tween.Stop();
    }

    /// <summary>
    /// Hover over the property in the editor to get the string value of that property.
    /// </summary>
    public void InterpolateProperty
    (
        NodePath property, 
        Variant finalValue, 
        float duration
    ) => tween.TweenProperty(target, property, finalValue, duration);

    public async Task AnimatePlatform
    (
        Vector2 initialValue, 
        Vector2 finalValue, 
        float width,
        float duration,
        int startDelay,
        Tween.TransitionType transType = Tween.TransitionType.Cubic,
        Tween.EaseType easeType = Tween.EaseType.InOut
    ) 
    {
        // tween.Repeat = true; // TODO: Godot 4 conversion
        InterpolateProperty("position", finalValue, duration);
        InterpolateProperty("position", initialValue, duration);
        await Task.Delay(startDelay * 1000);
        Start();
    }

    //public void IsActive() => tween.IsActive(); // TODO: Godot 4 conversion
    public void Start() => tween.Play();
    public void Pause() => tween.Stop();

    /// <summary>
    /// The name of the method passed must have Object @object, NodePath nodePath params
    /// </summary>
    public void OnCompleted(string method) => tween.Connect("tween_completed",new Callable(target,method));

    /// <summary>
    /// Emitted when all the animations in the tween have been completed.
    /// </summary>
    public void OnAllCompleted(string method) => tween.Connect("tween_all_completed",new Callable(target,method));
}
