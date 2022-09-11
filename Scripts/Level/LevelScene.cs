namespace Sankari;

public class LevelScene : Node
{
    public Camera Camera;
    public Dictionary<byte, OtherPlayer> OtherPlayers = new();

    public void PreInit()
    {
        var player = GetNode<Player>("Player");
        player.PreInit(this);
    }

    public override void _Ready()
    {
        GameManager.LevelScene = this;
        Camera = GetNode<Camera>("Camera");

        CreateLevelBounds();

        if (GameManager.Net.IsMultiplayer())
        {
            // TODO: Spawn other players
            var players = GameManager.UIPlayerList.Players;
            
            foreach (var player in players)
            {
                if (player.Key == GameManager.Net.Client.PeerId)
                    continue;

                var otherPlayer = Prefabs.OtherPlayer.Instance<OtherPlayer>();

                OtherPlayers.Add(player.Key, otherPlayer);

                AddChild(otherPlayer);
            }
        }
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
            new Vector2(Camera.LimitLeft - colliderThickness, (Camera.LimitTop + Camera.LimitBottom) / 2),
            new Vector2(colliderThickness, (Mathf.Abs(Camera.LimitTop) + Mathf.Abs(Camera.LimitBottom)) / 2)
        );

        // right
        CreateCollider
        (
            new Vector2(Camera.LimitRight + colliderThickness, (Camera.LimitTop + Camera.LimitBottom) / 2),
            new Vector2(colliderThickness, (Mathf.Abs(Camera.LimitTop) + Mathf.Abs(Camera.LimitBottom)) / 2)
        );

        // top
        CreateCollider
        (
            new Vector2((Camera.LimitLeft + Camera.LimitRight) / 2, Camera.LimitTop - colliderThickness),
            new Vector2(Mathf.Abs(Camera.LimitLeft) + (Mathf.Abs(Camera.LimitRight) / 2), colliderThickness)
        );
    }

    private void CreateCollider(Vector2 position, Vector2 extents)
    {
        var staticBody = new StaticBody2D();
        var collider = new CollisionShape2D();

        var shape = new RectangleShape2D();
        shape.Extents = extents;

        collider.Position = position;
        collider.Shape = shape;
        staticBody.AddChild(collider);

        AddChild(staticBody);
    }
}
