namespace Sankari;

/// <summary>
/// This class was created to attempt to simplify the process of creating C# for gamedev.
/// 
/// Limitations of this
/// - If there are 2 listeners for 1 event type then removing a listener will remove ALL the 
/// listeners for that event type (If this is undesired then one may consider using 
/// 'event Action<TArgs>' instead of this class)
/// - Can't define AddListener<T>(TEvent eventType, Action<T> action) so you don't know what
/// the types are on the subscriber end. If you try to define this, the godot game will
/// freeze on startup.
/// </summary>
/// <typeparam name="TEvent">The event type enum to be used. For example 'EventPlayer' enum.</typeparam>
public class EventManager<TEvent>
{
    private Dictionary<TEvent, List<Listener>> Listeners { get; set; } = new();

    /// <summary>
    /// The event type to be listened to
    /// </summary>
    public void AddListener(TEvent eventType, Action<object[]> action)
    {
        if (!Listeners.ContainsKey(eventType))
            Listeners.Add(eventType, new List<Listener>());

        Listeners[eventType].Add(new Listener(action));
    }

    /// <summary>
    /// Remove ALL listeners of type 'TEvent'
    /// </summary>
    public void RemoveAllListenersForEventType(TEvent eventType)
    {
        if (!Listeners.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in Listeners)
            for (int i = pair.Value.Count - 1; i >= 0; i--)
                if (pair.Key.Equals(eventType))
                    pair.Value.RemoveAt(i);
    }

    /// <summary>
    /// Remove ALL listeners from ALL event types
    /// </summary>
    public void RemoveAllListenersForAllEvents() 
    {
        foreach (TEvent eventType in Enum.GetValues(typeof(TEvent)))
            RemoveAllListenersForEventType(eventType);
    }

    /// <summary>
    /// Not sure if this is useful or not
    /// </summary>
    public void ClearListeners() => Listeners.Clear();

    /// <summary>
    /// Notify all listeners
    /// </summary>
    public void Notify(TEvent eventType, params object[] args)
    {
        if (!Listeners.ContainsKey(eventType))
            return;

        foreach (var listener in Listeners[eventType].ToList()) // if ToList() is not here then issue #137 will occur
            listener.Action(args);
    }

    private class Listener
    {
        public Action<object[]> Action { get; set; }
        public Listener(Action<object[]> action) => Action = action;
    }
}
