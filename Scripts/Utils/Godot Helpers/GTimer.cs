using Timer = Godot.Timer;

namespace Sankari;

public class GTimer
{
    private Timer Timer { get; } = new();

    public double TimeLeft 
    { 
        get { return Timer.TimeLeft; } 
    }

    public GTimer(Node target, string methodName, int delayMs = 1000, bool loop = true, bool autoStart = true)
    {
        Init(target, delayMs, loop, autoStart);
        Timer.Connect("timeout",new Callable(target,methodName));
    }

    private void Init(Node target, int delayMs, bool loop, bool autoStart)
    {
        Timer.WaitTime = delayMs / 1000f;
        Timer.OneShot = !loop;
        Timer.Autostart = autoStart;
        target.AddChild(Timer);
    }

    public bool IsActive() => Timer.TimeLeft != 0;
    public void SetDelay(float delay) => Timer.WaitTime = delay;
    public void SetDelayMs(int delayMs) => Timer.WaitTime = delayMs / 1000f;

    public void Start(float delay)
    {
        Timer.WaitTime = delay;
        Start();
    }
    public void StartMs(float delayMs)
    {
        Timer.WaitTime = delayMs / 1000;
        Start();
    }

    public void Start() => Timer.Start();
    public void Stop() => Timer.Stop();
    public void QueueFree() => Timer.QueueFree();
}
