using Timer = Godot.Timer;

namespace Sankari;

public class GTimer
{
	private Timer Timer { get; } = new();
	private Callable Callable { get; }

	public double TimeLeft
	{
		get { return Timer.TimeLeft; }
	}

	public GTimer(Node node, Callable callable, int delayMs = 1000, bool loop = true, bool autoStart = true)
	{
		Init(node, delayMs, loop, autoStart);
		Callable = callable;
		Timer.Connect("timeout", Callable);
	}

	public GTimer(Node target, string methodName, int delayMs = 1000, bool loop = true, bool autoStart = true)
		: this(target, new Callable(target, methodName), delayMs, loop, autoStart)
	{
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

	public void Stop()
	{
		Timer.Stop();
		Callable.Call();
	}

	public void QueueFree() => Timer.QueueFree();
}
