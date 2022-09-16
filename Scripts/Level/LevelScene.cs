namespace Sankari;

public partial class LevelScene : Node
{
    public LevelCamera camera;
    public Dictionary<byte, OtherPlayer> OtherPlayers = new();

    public void PreInit()
    {
        var player = GetNode<Player>("Player");
        player.PreInit(this);
    }

    public override void _Ready()
    {
        GameManager.LevelScene = this;
        camera = GetNode<LevelCamera>("Camera");

        CreateLevelBounds();

        if (GameManager.Net.IsMultiplayer())
        {
            var players = GameManager.UIPlayerList.Players;
            
            foreach (var player in players)
            {
                // WARN: Should not access Client.PeerId directly from another thread
                // Should not effect anything because this is only executed players.Count times once
                if (player.Key == GameManager.Net.Client.PeerId)
                    continue;

                var otherPlayer = Prefabs.OtherPlayer.Instantiate<OtherPlayer>();

                OtherPlayers.Add(player.Key, otherPlayer);

                AddChild(otherPlayer);
            }
        }

        GameManager.Notifications.AddListener(this, Event.OnGameClientLeft, (args) => 
        {
            var id = (byte)args[0];
            OtherPlayers[id].QueueFree();
            OtherPlayers.Remove(id);
        });
    }

    private void UpdateCheckpoints()
    {
        var checkpoints = GetNodeOrNull<Node2D>("Environment/Checkpoints");

        if (checkpoints == null)
            return;

        foreach (Checkpoint checkpoint in checkpoints.GetChildren())
        {
            // TODO: update checkpoints based on which were touched
        }
    }

    private void CreateLevelBounds()
    {
        var colliderThickness = 5;
        
        // left
        CreateCollider
        (
            new Vector2(camera.LimitLeft - colliderThickness, (camera.LimitTop + camera.LimitBottom) / 2),
            new Vector2(colliderThickness, (Mathf.Abs(camera.LimitTop) + Mathf.Abs(camera.LimitBottom)) / 2)
        );

        // right
        CreateCollider
        (
            new Vector2(camera.LimitRight + colliderThickness, (camera.LimitTop + camera.LimitBottom) / 2),
            new Vector2(colliderThickness, (Mathf.Abs(camera.LimitTop) + Mathf.Abs(camera.LimitBottom)) / 2)
        );

        // top
        CreateCollider
        (
            new Vector2((camera.LimitLeft + camera.LimitRight) / 2, camera.LimitTop - colliderThickness),
            new Vector2(Mathf.Abs(camera.LimitLeft) + (Mathf.Abs(camera.LimitRight) / 2), colliderThickness)
        );
    }

    private void CreateCollider(Vector2 position, Vector2 extents)
    {
        var staticBody = new StaticBody2D();
        var collider = new CollisionShape2D();

        var shape = new RectangleShape2D();
        shape.Size = extents;

        collider.Position = position;
        collider.Shape = shape;
        staticBody.AddChild(collider);

        AddChild(staticBody);
    }
}
