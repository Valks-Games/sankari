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

public enum PlayerEvent 
{
	OnJumped	
}

public class Notifications<TEvent>
{
    private Dictionary<TEvent, List<Listener>> Listeners { get; set; } = new();

    public void AddListener(Node sender, TEvent eventType, Action<object[]> action)
    {
        if (!Listeners.ContainsKey(eventType))
            Listeners.Add(eventType, new List<Listener>());

        Listeners[eventType].Add(new Listener(sender, action));
    }

    public void RemoveListener(Node sender, TEvent eventType)
    {
        if (!Listeners.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in Listeners)
            for (int i = pair.Value.Count - 1; i >= 0; i--)
                if (sender.GetInstanceId() == pair.Value[i].Sender.GetInstanceId())
                    pair.Value.RemoveAt(i);
    }

	public void RemoveListeners(Node sender) 
	{
		foreach (TEvent eventType in Enum.GetValues(typeof(TEvent)))
			RemoveListener(sender, eventType);
	}

    public void RemoveAllListeners() => Listeners.Clear();

    public void RemoveInvalidListeners()
    {
        var tempListeners = new Dictionary<TEvent, List<Listener>>();

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

    public void Notify(TEvent eventType, params object[] args)
    {
        if (!Listeners.ContainsKey(eventType))
            return;

        foreach (var listener in Listeners[eventType].ToList()) // if ToList() is not here then issue #137 will occur
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
