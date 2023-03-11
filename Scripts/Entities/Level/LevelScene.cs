namespace Sankari;

public partial class LevelScene : Node
{
    public LevelCamera Camera { get; set; }
    public Dictionary<byte, OtherPlayer> OtherPlayers { get; set; } = new();

    public void PreInit()
    {
        var player = GetNode<Player>("Player");
        player.PreInit(this);
    }

    public override void _Ready()
    {
        GameManager.LevelScene = this;
        Camera = GetNode<LevelCamera>("Camera");

        CreateLevelBounds();

        if (Net.IsMultiplayer())
        {
            var players = GameManager.UIPlayerList.Players;
            
            foreach (var player in players)
            {
                // WARN: Should not access Client.PeerId directly from another thread
                // Should not effect anything because this is only executed players.Count times once
                if (player.Key == Net.Client.PeerId)
                    continue;

                var otherPlayer = Prefabs.OtherPlayer.Instantiate<OtherPlayer>();

                OtherPlayers.Add(player.Key, otherPlayer);

                AddChild(otherPlayer);
            }
        }

		Events.Generic.AddListener(EventGeneric.OnGameClientLeft, (args) => 
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
		// I don't know if this is 100% correct but it seems to do the job for now
        var thickness = 5;
		var height = Mathf.Abs(Camera.LimitTop) + Mathf.Abs(Camera.LimitBottom);
		var width = Mathf.Abs(Camera.LimitLeft) + Mathf.Abs(Camera.LimitRight);

		// left
		CreateCollider
		(
			new Vector2(Camera.LimitLeft - thickness, Camera.LimitBottom - height / 2),
			new Vector2(thickness, height)
		);

		// right
		CreateCollider
		(
			new Vector2(Camera.LimitRight + thickness, Camera.LimitBottom - height / 2),
			new Vector2(thickness, height)
		);

        // top
        CreateCollider
        (
            new Vector2(width / 2, Camera.LimitTop - thickness),
            new Vector2(width, thickness)
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
