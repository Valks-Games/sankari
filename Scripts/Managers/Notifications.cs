namespace Sankari;

public enum Event
{
    OnKeyboardInput,
    OnMouseButtonInput,
    OnMouseMotionInput,
    OnJoypadButtonInput,
    OnSceneChanged,
    OnGameClientStopped,
    OnGameClientConnected,
    OnLevelLoaded,
    OnGameClientLeft,
    OnGameClientJoined,
    OnReceivePlayersFromServer
}

public static class Notifications
{
    private static Dictionary<Event, List<AudioListener3D>> Listeners { get; set; } = new();

    public static void AddListener(Node sender, Event eventType, Action<object[]> action)
    {
        if (!Listeners.ContainsKey(eventType))
            Listeners.Add(eventType, new List<AudioListener3D>());

        Listeners[eventType].Add(new AudioListener3D(sender, action));
    }

    public static void RemoveListener(Node sender, Event eventType)
    {
        if (!Listeners.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in Listeners)
            for (int i = pair.Value.Count - 1; i >= 0; i--)
                if (sender.GetInstanceId() == pair.Value[i].Sender.GetInstanceId())
                    pair.Value.RemoveAt(i);
    }

    public static void RemoveAllListeners() => Listeners.Clear();

    public static void RemoveInvalidListeners()
    {
        var tempListeners = new Dictionary<Event, List<AudioListener3D>>();

        foreach (var pair in Listeners)
        {
            for (int i = pair.Value.Count - 1; i >= 0; i--)
            {
                if (!Godot.Object.IsInstanceValid(pair.Value[i].Sender))
                    pair.Value.RemoveAt(i);
            }

            if (pair.Value.Count > 0)
                tempListeners.Add(pair.Key, pair.Value);
        }

        Listeners = new(tempListeners);
    }

    public static void Notify(Event eventType, params object[] args)
    {
        if (!Listeners.ContainsKey(eventType))
            return;

        foreach (var listener in Listeners[eventType].ToList()) // if ToList() is not here then issue #137 will occur
            listener.Action(args);
    }

    private class AudioListener3D
    {
        public Node Sender { get; set; }
        public Action<object[]> Action { get; set; }

        public AudioListener3D(Node sender, Action<object[]> action)
        {
            Sender = sender;
            Action = action;
        }
    }
}
