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
    OnReceivePlayersFromServer,
	OnCoinPickup
}

public enum EventPlayer 
{
	OnJumped,
	OnDied,
	OnDash
}

public class EventManager<TEvent>
{
    private Dictionary<TEvent, List<Listener>> Listeners { get; set; } = new();

    public void AddListener(string sender, TEvent eventType, Action<object[]> action)
    {
        if (!Listeners.ContainsKey(eventType))
            Listeners.Add(eventType, new List<Listener>());

        Listeners[eventType].Add(new Listener(sender, action));
    }

    public void RemoveListener(string sender, TEvent eventType)
    {
        if (!Listeners.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in Listeners)
            for (int i = pair.Value.Count - 1; i >= 0; i--)
                if (sender == pair.Value[i].Sender)
                    pair.Value.RemoveAt(i);
    }

	public void RemoveListeners(string sender) 
	{
		foreach (TEvent eventType in Enum.GetValues(typeof(TEvent)))
			RemoveListener(sender, eventType);
	}

    public void RemoveAllListeners() => Listeners.Clear();

    public void Notify(TEvent eventType, params object[] args)
    {
        if (!Listeners.ContainsKey(eventType))
            return;

        foreach (var listener in Listeners[eventType].ToList()) // if ToList() is not here then issue #137 will occur
            listener.Action(args);
    }

    private class Listener
    {
        public string Sender { get; set; }
        public Action<object[]> Action { get; set; }

        public Listener(string sender, Action<object[]> action)
        {
            Sender = sender;
            Action = action;
        }
    }
}
