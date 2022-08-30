namespace Sankari;

public class GTween 
{
    private Tween tween;
    private Node target;

    /// <summary>
    /// Should the tween animation loop? Note that if Repeat is set to false than the values set in InterpolateProperty
    /// will be wiped when the animation finishes.
    /// </summary>
    public bool Repeat
    {
        get { return tween.Repeat; }
        set { tween.Repeat = value; }
    }

    public GTween(Node target)
    {
        tween = new Tween();
        tween.ProcessPriority = (int)Tween.TweenProcessMode.Physics;
        this.target = target;
        this.target.AddChild(tween);
    }

    /// <summary>
    /// Hover over the property in the editor to get the string value of that property.
    /// </summary>
    public bool InterpolateProperty
    (
        NodePath property, 
        object initialValue, 
        object finalValue, 
        float duration, 
        float delay = 0,
        Tween.TransitionType transType = Tween.TransitionType.Cubic, 
        Tween.EaseType easeType = Tween.EaseType.InOut
    ) => tween.InterpolateProperty(target, property, initialValue, finalValue, duration, transType, easeType, delay);

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
        tween.Repeat = true;
        InterpolateProperty("position", initialValue, finalValue, duration, 0, transType, easeType);
        InterpolateProperty("position", finalValue, initialValue, duration, duration, transType, easeType);
        await Task.Delay(startDelay * 1000);
        Start();
    }

    public void IsActive() => tween.IsActive();
    public void Start() => tween.Start();
    public void Pause() => tween.StopAll();
    public void Resume() => tween.ResumeAll();

    /// <summary>
    /// The name of the method passed must have Object @object, NodePath nodePath params
    /// </summary>
    public void OnCompleted(string method) => tween.Connect("tween_completed", target, method);

    /// <summary>
    /// Emitted when all the animations in the tween have been completed.
    /// </summary>
    public void OnAllCompleted(string method) => tween.Connect("tween_all_completed", target, method);
}