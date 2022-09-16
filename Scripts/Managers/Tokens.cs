namespace Sankari;

public partial class Tokens
{
    private readonly Dictionary<string, CancellationTokenSource> cts = new();

    public CancellationTokenSource Create(string name, int timeout = 0)
    {
        Cancel(name);
        cts[name] = new CancellationTokenSource();
        if (timeout > 0) cts[name].CancelAfter(timeout);
        return cts[name];
    }

    public bool Cancelled(string name) =>
        cts.ContainsKey(name) && cts[name].IsCancellationRequested;

    public void Cancel(string name)
    {
        if (cts.ContainsKey(name))
        {
            try
            {
                cts[name].Cancel();
            }
            catch (ObjectDisposedException)
            {
                Logger.LogWarning($"Token '{name}' could not be cancelled because it has been disposed");
            }
        }
    }

    public void Cleanup() => cts.Values.ForEach(x =>
    {
        x.Cancel();
        x.Dispose();
    });
}