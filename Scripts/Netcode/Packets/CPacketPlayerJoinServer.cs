namespace Sankari.Netcode;

public class CPacketPlayerJoinServer : APacketClient
{
    public string Username { get; set; }
    public bool Host { get; set; }
    public string Password { get; set; } = "";

    public override void Write(PacketWriter writer)
    {
        writer.Write(Username);
        writer.Write(Host);
        writer.Write(Password);
    }

    public override void Read(PacketReader reader)
    {
        Username = reader.ReadString();
        Host = reader.ReadBool();
        Password = reader.ReadString();
    }

    public override void Handle(GameServer server, ENet.Peer peer)
    {
        if (server.Players.ContainsKey((byte)peer.ID)) 
        {
            server.Kick(peer.ID, DisconnectOpcode.Kicked);
            Logger.LogWarning($"Player with username '{Username}' tried to join but they are on the server already so they were kicked");
            return;
        }

        server.Players[(byte)peer.ID] = new DataPlayer {
            Username = Username,
            Host = Host
        };

        Logger.Log($"Player with username '{Username}' joined");

        // notify other players of this player
        GameManager.Net.Server.SendToOtherPlayers(peer.ID, ServerPacketOpcode.PlayerJoined);
    }
}
