namespace MarioLikeGame;

public class GTween 
{
    private Tween _tween;
    private Node _target;

    /// <summary>
    /// Should the tween animation loop?
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
        Tween.TransitionType transType = Tween.TransitionType.Linear, 
        Tween.EaseType easeType = Tween.EaseType.InOut, 
        float delay = 0
    ) => _tween.InterpolateProperty(_target, property, initialValue, finalValue, duration, transType, easeType, delay);

    public void Start() => _tween.Start();

    /// <summary>
    /// The name of the method passed must have Object @object, NodePath nodePath params
    /// </summary>
    public void OnCompleted(string method) => _tween.Connect("tween_completed", _target, method);
}