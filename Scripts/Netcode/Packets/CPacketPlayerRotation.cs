using Sankari.Netcode.Server;

namespace Sankari.Netcode;

public class CPacketPlayerRotation : APacketClient
{
    public float Rotation { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((float)System.Math.Round(Rotation, 1));
    }

    public override void Read(PacketReader reader)
    {
        Rotation = reader.ReadFloat();
    }

    public override void Handle(GameServer server, ENet.Peer peer)
    {
        var player = server.Players[(byte)peer.ID];
        player.Rotation = Rotation;
    }
}
