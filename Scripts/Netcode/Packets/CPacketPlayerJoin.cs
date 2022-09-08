namespace Sankari.Netcode;

/// <summary>
/// Tell server that we joined the server as a new player. 
/// Also send another packet to inform everyone else playing with us that we are playing.
/// </summary>
public class CPacketPlayerJoin : APacketClient
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

    public override void Handle(ENet.Peer peer)
    {
        var server = GameManager.Net.Server;

        if (server.Players.ContainsKey((byte)peer.ID)) 
        {
            server.Kick(peer.ID, DisconnectOpcode.Kicked);
            Logger.LogWarning($"Player with username '{Username}' tried to join but they are on the server already so they were kicked");
            return;
        }

        // notify joining player of all players in the server
        server.Send(ServerPacketOpcode.PlayersOnServer, new SPacketPlayersOnServer 
        {
            Usernames = server.Players.Select(x => x.Value.Username).ToArray()
        }, peer);

        // notify other players that this player is joining
        server.SendToOtherPlayers(peer.ID, ServerPacketOpcode.PlayerJoinLeave, new SPacketPlayerJoinLeave 
        {
            Username = Username,
            Joining = true
        });

        server.Players[(byte)peer.ID] = new DataPlayer {
            Username = Username,
            Host = Host
        };

        Logger.Log($"Player with username '{Username}' joined");
    }
}
