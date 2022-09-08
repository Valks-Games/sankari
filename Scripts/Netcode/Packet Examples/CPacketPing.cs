using Sankari.Netcode.Server;

namespace Sankari.Netcode;

public class CPacketPing : APacketClient
{
    public override void Handle(ENet.Peer peer)
    {
        GameManager.Net.Server.Send(ServerPacketOpcode.Pong, peer);
    }
}
