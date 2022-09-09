namespace Sankari;

using System.Timers;
using Timer = System.Timers.Timer;
using Object = System.Object;

public class STimer : IDisposable
{
    private Timer Timer { get; set; }

    public STimer(double delayMs, Action action, bool enabled = true, bool autoreset = true)
    {
        void Callback(Object source, ElapsedEventArgs e) => action();
        Timer = new Timer(delayMs);
        Timer.Enabled = enabled;
        Timer.AutoReset = autoreset;
        Timer.Elapsed += Callback;
    }

    public void Start() => Timer.Enabled = true;
    public void Stop() => Timer.Enabled = false;
    public void SetDelay(double delayMs) => Timer.Interval = delayMs;

    public void Dispose() => Timer.Dispose();
}