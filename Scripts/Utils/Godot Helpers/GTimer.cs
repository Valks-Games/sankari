using Timer = Godot.Timer;

namespace Sankari;

public partial class GTimer
{
    private readonly Timer timer = new();

    public double TimeLeft 
    { 
        get { return timer.TimeLeft; } 
    }

    public GTimer(Node target, string methodName, int delayMs = 1000, bool loop = true, bool autoStart = true)
    {
        Init(target, delayMs, loop, autoStart);
        timer.Connect("timeout",new Callable(target,methodName));
    }

    private void Init(Node target, int delayMs, bool loop, bool autoStart)
    {
        timer.WaitTime = delayMs / 1000f;
        timer.OneShot = !loop;
        timer.Autostart = autoStart;
        target.AddChild(timer);
    }

    public bool IsActive() => timer.TimeLeft != 0;
    public void SetDelay(float delay) => timer.WaitTime = delay;
    public void SetDelayMs(int delayMs) => timer.WaitTime = delayMs / 1000f;

    public void Start(float delay)
    {
        timer.WaitTime = delay;
        Start();
    }
    public void StartMs(float delayMs)
    {
        timer.WaitTime = delayMs / 1000;
        Start();
    }

    public void Start() => timer.Start();
    public void Stop() => timer.Stop();
    public void QueueFree() => timer.QueueFree();
}
