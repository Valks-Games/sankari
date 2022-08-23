namespace Sankari;

public class GTween 
{
    private Tween _tween;
    private Node _target;

    /// <summary>
    /// Should the tween animation loop? Note that if Repeat is set to false than the values set in InterpolateProperty
    /// will be wiped when the animation finishes.
    /// </summary>
    public bool Repeat
    {
        get { return _tween.Repeat; }
        set { _tween.Repeat = value; }
    }

    public GTween(Node target)
    {
        _tween = new Tween();
        _target = target;
        _target.AddChild(_tween);
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
    ) => _tween.InterpolateProperty(_target, property, initialValue, finalValue, duration, transType, easeType, delay);

    public async Task AnimatePlatform
    (
        object initialValue, 
        object finalValue, 
        float duration,
        int startDelay,
        Tween.TransitionType transType = Tween.TransitionType.Cubic,
        Tween.EaseType easeType = Tween.EaseType.InOut
    ) 
    {
        _tween.Repeat = true;
        InterpolateProperty("position", initialValue, finalValue, duration, 0, transType, easeType);
        InterpolateProperty("position", finalValue, initialValue, duration, duration, transType, easeType);
        await Task.Delay(startDelay * 1000);
        Start();
    }

    public void IsActive() => _tween.IsActive();
    public void Start() => _tween.Start();
    public void Pause() => _tween.StopAll();
    public void Resume() => _tween.ResumeAll();

    /// <summary>
    /// The name of the method passed must have Object @object, NodePath nodePath params
    /// </summary>
    public void OnCompleted(string method) => _tween.Connect("tween_completed", _target, method);
}