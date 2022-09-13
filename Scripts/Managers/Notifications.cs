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
    OnGameClientJoined
}

public class Notifications
{
    private Dictionary<Event, List<Listener>> listeners = new();

    public void AddListener(Node sender, Event eventType, Action<object[]> action)
    {
        if (!listeners.ContainsKey(eventType))
            listeners.Add(eventType, new List<Listener>());

        listeners[eventType].Add(new Listener(sender, action));
    }

    public void RemoveListener(Node sender, Event eventType)
    {
        if (!listeners.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in listeners)
            for (int i = pair.Value.Count - 1; i >= 0; i--)
                if (sender.GetInstanceId() == pair.Value[i].Sender.GetInstanceId())
                    pair.Value.RemoveAt(i);
    }

    public void RemoveAllListeners() => listeners.Clear();

    public void RemoveInvalidListeners()
    {
        var tempListeners = new Dictionary<Event, List<Listener>>();

        foreach (var pair in listeners)
        {
            for (int i = pair.Value.Count - 1; i >= 0; i--)
            {
                if (!Godot.Object.IsInstanceValid(pair.Value[i].Sender))
                    pair.Value.RemoveAt(i);
            }

            if (pair.Value.Count > 0)
                tempListeners.Add(pair.Key, pair.Value);
        }

        listeners = new(tempListeners);
    }

    public void Notify(Event eventType, params object[] args)
    {
        if (!listeners.ContainsKey(eventType))
            return;

        foreach (var listener in listeners[eventType].ToList()) // if ToList() is not here then issue #137 will occur
            listener.Action(args);
    }

    private class Listener
    {
        public Node Sender { get; set; }
        public Action<object[]> Action { get; set; }

        public Listener(Node sender, Action<object[]> action)
        {
            Sender = sender;
            Action = action;
        }
    }
}
