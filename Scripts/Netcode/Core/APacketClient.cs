using Sankari.Netcode.Server;

namespace Sankari.Netcode;

public abstract class APacketClient : APacket
{
    /// <summary>
    /// The packet handled server-side
    /// </summary>
    /// <param name="peer">The client peer</param>
    public abstract void Handle(GameServer server, ENet.Peer peer);
}
