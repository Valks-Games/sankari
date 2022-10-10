namespace Sankari.Netcode;

public static class NetIntervals 
{
    public static int HEARTBEAT { get; set; } = 75;
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
