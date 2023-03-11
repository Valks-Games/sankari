using System.Runtime;

namespace Sankari;

/// <summary>
/// This class was created to attempt to simplify the process of creating C# for gamedev.
/// 
/// Limitations of this
/// - If there are 2 listeners for 1 event type then removing a listener will remove ALL the 
/// listeners for that event type (If this is undesired then one may consider using 
/// 'event Action<TArgs>' instead of this class)
/// </summary>
/// <typeparam name="TEvent">The event type enum to be used. For example 'EventPlayer' enum.</typeparam>
public class EventManager<TEvent>
{
    private Dictionary<TEvent, List<object>> Actions { get; set; } = new();

    /// <summary>
    /// The event type to be listened to
    /// </summary>
    public void AddListener(TEvent eventType, Action<object[]> action) =>
        AddListener<object[]>(eventType, action);

    public void AddListener<T>(TEvent eventType, Action<T> action, string id = "")
    {
        if (!Actions.ContainsKey(eventType))
            Actions.Add(eventType, new List<object>());

        Actions[eventType].Add(action);
    }

    /// <summary>
    /// Remove ALL listeners of type 'TEvent'
    /// </summary>
    public void RemoveAllListenersForEventType(TEvent eventType)
    {
        if (!Actions.ContainsKey(eventType))
            throw new InvalidOperationException($"Tried to remove listener of event type '{eventType}' from an event type that has not even been defined yet");

        foreach (var pair in Actions)
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
    public void ClearListeners() => Actions.Clear();

    /// <summary>
    /// Notify all listeners
    /// </summary>
    public void Notify(TEvent eventType, params object[] args)
    {
        if (!Actions.ContainsKey(eventType))
            return;

        foreach (dynamic action in Actions[eventType].ToList()) // if ToList() is not here then issue #137 will occur
            action(args);
    }
}
