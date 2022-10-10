namespace Sankari.Netcode;

public enum ClientGameInfo
{
    PlayerJoin
}

/// <summary>
/// Tell server that we joined the server as a new player. 
/// Also send another packet to inform everyone else playing with us that we are playing.
/// </summary>
public class CPacketGameInfo : APacketClient
{
    public ClientGameInfo ClientGameInfo { get; set; }

    // PlayerJoin
    public string Username { get; set; }
    public bool Host { get; set; }
    public string Password { get; set; } = "";

    public override void Write(PacketWriter writer)
    {
        writer.Write((ushort)ClientGameInfo);

        switch (ClientGameInfo)
        {
            case ClientGameInfo.PlayerJoin:
                writer.Write(Username);
                writer.Write(Host);
                writer.Write(Password);
                break;
        }
    }

    public override void Read(PacketReader reader)
    {
        ClientGameInfo = (ClientGameInfo)reader.ReadUShort();

        Logger.Log($"[Server] Received: {ClientGameInfo}");

        switch (ClientGameInfo)
        {
            case ClientGameInfo.PlayerJoin:
                Username = reader.ReadString();
                Host = reader.ReadBool();
                Password = reader.ReadString();
                break;
        }
    }

    private ENet.Peer peer;
    private GameServer server;

    public override void Handle(ENet.Peer peer)
    {
        this.peer = peer;
        this.server = Net.Server;

        switch (ClientGameInfo)
        {
            case ClientGameInfo.PlayerJoin:
                HandlePlayerJoin();
                break;
        }
    }

    private void HandlePlayerJoin()
    {
        if (server.Players.ContainsKey((byte)peer.ID))
        {
            server.Kick(peer.ID, DisconnectOpcode.Kicked);
            Logger.LogWarning($"[Server] Player with username '{Username}' tried to join but they are on the server already so they were kicked");
            return;
        }

        if (!Host)
        {
            // notify joining player of all players in the server
            server.Send(ServerPacketOpcode.GameInfo, new SPacketGameInfo
            {
                ServerGameInfo = ServerGameInfo.PlayersOnServer,
                Usernames = server.Players.ToDictionary(x => x.Key, x => x.Value.Username)
            }, peer);

            // notify joining player of their peer id
            server.Send(ServerPacketOpcode.GameInfo, new SPacketGameInfo
            {
                ServerGameInfo = ServerGameInfo.PeerId,
                PeerId = (byte)peer.ID
            }, peer);

            // notify joining player of current map position
            server.Send(ServerPacketOpcode.GameInfo, new SPacketGameInfo
            {
                ServerGameInfo = ServerGameInfo.MapPosition,
                MapPosition = GameManager.Map.CurMapPos
            }, peer);
        }

        // track joining player in server player dictionary
        server.Players[(byte)peer.ID] = new DataPlayer
        {
            Username = Username,
            Host = Host
        };

        // notify other players that this player has joined
        server.SendToAllPlayers(ServerPacketOpcode.GameInfo, new SPacketGameInfo
        {
            ServerGameInfo = ServerGameInfo.PlayerJoinLeave,
            Username = Username,
            Joining = true,
            Id = (byte)peer.ID
        });

        if (Host)
            server.HostId = peer.ID;

        Logger.Log($"[Server] Player with username '{Username}' (id {peer.ID}) joined");
    }
}
