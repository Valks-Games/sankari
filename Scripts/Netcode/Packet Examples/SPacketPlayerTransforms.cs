
using Sankari.Netcode.Client;

namespace Sankari.Netcode;

public class SPacketPlayerTransforms : APacketServer
{
    public Dictionary<byte, DataTransform> PlayerTransforms { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)PlayerTransforms.Count);
        PlayerTransforms.ForEach(pair =>
        {
            var transform = pair.Value;

            writer.Write(pair.Key); // id
            writer.Write(transform.Position);
            writer.Write((float)System.Math.Round(transform.Rotation, 1));
        });
    }

    public override void Read(PacketReader reader)
    {
        PlayerTransforms = new Dictionary<byte, DataTransform>();
        var count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            var id = reader.ReadByte();
            var pos = reader.ReadVector2();
            var rot = reader.ReadFloat();

            PlayerTransforms[id] = new DataTransform
            {
                Position = pos,
                Rotation = rot
            };
        }
    }

#if CLIENT
    public override async Task Handle(GameClient client, Managers managers)
    {
        /*if (SceneManager.InGame())
            SceneManager.GetActiveSceneScript<Game.SceneGame>().UpdatePlayerPositions(PlayerTransforms);*/
        await Task.FromResult(1);
    }
#endif
}
