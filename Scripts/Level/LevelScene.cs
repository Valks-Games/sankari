namespace Sankari;

public class LevelScene : Node
{
    public GameManager GameManager { get; private set; }
    public Camera Camera;

    public void PreInit(GameManager gameManager)
    {
        GameManager = gameManager;
        
        InitPlayer();
        InitGameObjects();
    }

    public override void _Ready()
    {
        Camera = GetNode<Camera>("Camera");

        CreateLevelBounds();
    }

    private void InitPlayer()
    {
        var player = GetNode<Player>("Player");
        player.PreInit(this);
    }

    private void InitGameObjects()
    {
        InitCoins();
    }

    private void InitCoins()
    {
        var coins = GetNodeOrNull("Environment/Coins");

        if (coins == null)
            return;

        foreach (Coin coin in coins.GetChildren())
            coin.PreInit(GameManager);
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
            new Vector2((Mathf.Abs(Camera.LimitLeft) + Mathf.Abs(Camera.LimitRight) / 2), colliderThickness)
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
