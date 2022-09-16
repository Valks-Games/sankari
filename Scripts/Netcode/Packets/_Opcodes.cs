namespace Sankari.Netcode;

public static class NetIntervals 
{
    public const int HEARTBEAT = 75;
}

// Received from Game Client
public enum ClientPacketOpcode
{
    GameInfo,
    PlayerPosition
}

// Sent to Game Client
public enum ServerPacketOpcode
{
    GameInfo,
    PlayerPositions
}

public enum DisconnectOpcode
{
    Disconnected,
    Timeout,
    Maintenance,
    Restarting,
    Stopping,
    Kicked,
    Banned
}
