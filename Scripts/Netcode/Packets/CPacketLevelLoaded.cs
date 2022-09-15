using System.Dynamic;

namespace Sankari.Netcode;

public class CPacketLevelLoaded : APacketClient, ITrackablePacket
{
    public Guid UniqueId { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write(UniqueId.ToByteArray());
    }

    public override void Read(PacketReader reader)
    {
        UniqueId = reader.Read<Guid>();
    }

    public override void Handle(ENet.Peer peer)
    {
        Logger.Log(UniqueId.ToString());
    }
}