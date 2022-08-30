using Timer = Godot.Timer;

namespace Sankari;

public class GTimer
{
    private readonly Timer _timer = new Timer();

    public float TimeLeft 
    { 
        get { return _timer.TimeLeft; } 
    }

    public GTimer(Node target, string methodName, int delayMs = 1000, bool loop = true, bool autoStart = true)
    {
        Init(target, delayMs, loop, autoStart);
        _timer.Connect("timeout", target, methodName);
    }

    private void Init(Node target, int delayMs, bool loop, bool autoStart)
    {
        _timer.WaitTime = delayMs / 1000f;
        _timer.OneShot = !loop;
        _timer.Autostart = autoStart;
        target.AddChild(_timer);
    }

    public bool IsActive() => _timer.TimeLeft != 0;
    public void SetDelay(float delay) => _timer.WaitTime = delay;
    public void SetDelayMs(int delayMs) => _timer.WaitTime = delayMs / 1000f;

    public void Start(float delay)
    {
        _timer.WaitTime = delay;
        Start();
    }
    public void StartMs(float delayMs)
    {
        _timer.WaitTime = delayMs / 1000;
        Start();
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
    public void QueueFree() => _timer.QueueFree();
}