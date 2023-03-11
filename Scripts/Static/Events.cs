namespace Sankari;

public static class Events
{
    public static EventManager<EventGeneric> Generic { get; private set; } = new();
    public static EventManager<EventPlayer>  Player  { get; private set; } = new();
}

public enum EventGeneric
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
    OnCoinPickup,
    OnMapLoaded
}

public enum EventPlayer
{
    OnJump,
    OnDied,
    OnDash
}
