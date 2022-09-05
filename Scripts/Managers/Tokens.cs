namespace Sankari;

public class Tokens
{
    private Dictionary<string, CancellationTokenSource> _cts = new();

    public CancellationTokenSource Create(string name, int timeout = 0)
    {
        Cancel(name);
        _cts[name] = new CancellationTokenSource();
        if (timeout > 0) _cts[name].CancelAfter(timeout);
        return _cts[name];
    }

    public bool Cancelled(string name) =>
        _cts.ContainsKey(name) && _cts[name].IsCancellationRequested;

    public void Cancel(string name)
    {
        if (_cts.ContainsKey(name))
        {
            try
            {
                _cts[name].Cancel();
            }
            catch (ObjectDisposedException)
            {
                Logger.LogWarning($"Token '{name}' could not be cancelled because it has been disposed");
            }
        }
    }

    public void Cleanup() => _cts.Values.ForEach(x =>
    {
        x.Cancel();
        x.Dispose();
    });
}