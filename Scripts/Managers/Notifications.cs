namespace Sankari;

public enum Event
{
    OnKeyboardInput,
    OnMouseButtonInput,
    OnMouseMotionInput,
    OnJoypadButtonInput,
    OnSceneChanged,
    OnGameClientStopped,
    OnGameClientConnected
}

public class Notifications
{
    private Dictionary<Event, List<Listener>> _listeners = new();

    public void AddListener(Node sender, Event eventType, Action<Node, object[]> action)
    {
        if (!_listeners.ContainsKey(eventType))
            _listeners.Add(eventType, new List<Listener>());

        _listeners[eventType].Add(new Listener(sender, action));
    }

    public void RemoveListener(Node sender, Event eventType)
    {
        if (!_listeners.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in _listeners)
            for (int i = pair.Value.Count - 1; i >= 0; i--)
                if (sender.GetInstanceId() == pair.Value[i].Sender.GetInstanceId())
                    pair.Value.RemoveAt(i);
    }

    public void RemoveAllListeners() => _listeners.Clear();

    public void RemoveInvalidListeners()
    {
        var tempListeners = new Dictionary<Event, List<Listener>>();

        foreach (var pair in _listeners)
        {
            for (int i = pair.Value.Count - 1; i >= 0; i--)
            {
                if (!Godot.Object.IsInstanceValid(pair.Value[i].Sender))
                    pair.Value.RemoveAt(i);
            }

            if (pair.Value.Count > 0)
                tempListeners.Add(pair.Key, pair.Value);
        }

        _listeners = new(tempListeners);
    }

    public void Notify(Node sender, Event eventType, params object[] args)
    {
        if (!_listeners.ContainsKey(eventType))
            return;

        foreach (var listener in _listeners[eventType].ToList()) // if ToList() is not here then issue #137 will occur
            listener.Action(sender, args);
    }

    private class Listener
    {
        public Node Sender { get; set; }
        public Action<Node, object[]> Action { get; set; }

        public Listener(Node sender, Action<Node, object[]> action)
        {
            Sender = sender;
            Action = action;
        }
    }
}
