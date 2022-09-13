namespace Sankari;

public class SPacketPlayerPositions : APacketServer
{
    public Dictionary<byte, Vector2> PlayerPositions { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((byte)PlayerPositions.Count);
        foreach (var player in PlayerPositions) 
        {
            writer.Write((byte)player.Key);
            writer.Write(player.Value);
        }
        
    }

    public override void Read(PacketReader reader)
    {
        PlayerPositions = new Dictionary<byte, Vector2>();

        var playerCount = reader.ReadByte();
        for (int i = 0; i < playerCount; i++) 
        {
            var key = reader.ReadByte();
            var value = reader.ReadVector2();

            PlayerPositions[key] = value;
        }
    }

    public override async Task Handle()
    {
        if (GameManager.LevelScene == null) 
        {
            Logger.LogWarning("Level scene is null");
            return;
        }

        if (!GameManager.LevelScene.LevelSceneReady) 
        {
            Logger.LogWarning("Level scene is not ready");
            return;
        }

        foreach (var player in PlayerPositions)
        {
            var playerId = player.Key;
            var playerPos = player.Value;

            // other client
            // id 1 not present in dictionary
            GameManager.LevelScene.OtherPlayers[playerId].Position = playerPos;
        }

        await Task.FromResult(0);
    }
}
