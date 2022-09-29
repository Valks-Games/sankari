namespace Sankari;

public static class Tokens
{
    private static Dictionary<string, CancellationTokenSource> Cts { get; } = new();

    public static CancellationTokenSource Create(string name, int timeout = 0)
    {
        Cancel(name);
        Cts[name] = new CancellationTokenSource();
        if (timeout > 0) Cts[name].CancelAfter(timeout);
        return Cts[name];
    }

    public static bool Cancelled(string name) =>
        Cts.ContainsKey(name) && Cts[name].IsCancellationRequested;

    public static void Cancel(string name)
    {
        if (Cts.ContainsKey(name))
        {
            try
            {
                Cts[name].Cancel();
            }
            catch (ObjectDisposedException)
            {
                Logger.LogWarning($"Token '{name}' could not be cancelled because it has been disposed");
            }
        }
    }

    public static void Cleanup() => Cts.Values.ForEach(x =>
    {
        x.Cancel();
        x.Dispose();
    });
}