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

        if (PlayerPositions.Count == 0) 
        {
            Logger.LogWarning("Player positions count is 0");
            return;
        }

        GameManager.LevelScene.OtherPlayers.First().Value.Position = PlayerPositions[0];

        /*foreach (var player in PlayerPositions)
        {
            if (GameManager.Net.Client.PeerId == player.Key)
                continue;

            Logger.Log(player.Value);

            GameManager.LevelScene.OtherPlayers[player.Key].Position = player.Value;
        }*/

        await Task.FromResult(0);
    }
}