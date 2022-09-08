
using Sankari.Netcode.Client;

namespace Sankari.Netcode;

public class SPacketGame : APacketServer
{
    private GameOpcode GameOpcode { get; set; }
    public Dictionary<ushort, DataEnemy> Enemies { get; set; }

    public SPacketGame() { } // need for reflection

    public SPacketGame(GameOpcode opcode)
    {
        GameOpcode = opcode;
    }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)GameOpcode);

        if (GameOpcode == GameOpcode.EnemiesSpawned)
        {
            writer.Write((ushort)Enemies.Count);
            foreach (var pair in Enemies)
            {
                writer.Write((ushort)pair.Key); // id
                writer.Write(pair.Value.Position);
            }
        }
    }

    public override void Read(PacketReader reader)
    {
        GameOpcode = (GameOpcode)reader.ReadByte();

        if (GameOpcode == GameOpcode.EnemiesSpawned)
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
    }

#if CLIENT
    public override async Task Handle(GameClient client, Managers managers)
    {
        /*var sceneGameScript = SceneManager.GetActiveSceneScript<Game.SceneGame>();

        foreach (var pair in Enemies)
        {
            var enemy = Prefabs.Enemy.Instance<GodotModules.Netcode.Server.Enemy>();
            enemy.Position = pair.Value.Position;
            enemy.SetPlayers(sceneGameScript.Players);
            sceneGameScript.AddChild(enemy);
            sceneGameScript.Enemies[pair.Key] = enemy;
        }*/

        await Task.FromResult(1);
    }
#endif
}
