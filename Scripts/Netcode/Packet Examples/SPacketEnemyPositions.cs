
using Sankari.Netcode.Client;

namespace Sankari.Netcode;

public class SPacketEnemyPositions : APacketServer
{
    private GameOpcode GameOpcode { get; set; }
    public Dictionary<ushort, DataEnemy> Enemies { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((ushort)Enemies.Count);

        foreach (var pair in Enemies)
        {
            writer.Write((ushort)pair.Key); // id
            writer.Write(pair.Value.Position);
        }
    }

    public override void Read(PacketReader reader)
    {
        Enemies = new Dictionary<ushort, DataEnemy>();
        var count = reader.ReadUShort();
        for (int i = 0; i < count; i++)
        {
            var id = reader.ReadUShort();
            var pos = reader.ReadVector2();

            Enemies.Add(id, new DataEnemy
            {
                Position = pos
            });
        }
    }

#if CLIENT
    public override async Task Handle(GameClient client, Managers managers)
    {
        //var sceneGameScript = SceneManager.GetActiveSceneScript<Game.SceneGame>();

        //sceneGameScript.EnemyTransformQueue.Add(Enemies);

        /*foreach (var pair in Enemies)
        {
            var enemy = sceneGameScript.Enemies[pair.Key];
            if (enemy.Position.DistanceTo(pair.Value.Position) > 100) // TODO: Lerp
                sceneGameScript.Enemies[pair.Key].Position = pair.Value.Position;
        }*/

        await Task.FromResult(1);
    }
#endif
}
