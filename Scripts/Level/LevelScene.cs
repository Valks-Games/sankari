namespace Sankari;

public class LevelScene : Node
{
    [Export] protected readonly NodePath NodePathCoinSprite;
    [Export] protected readonly NodePath NodePathLabelCoins;

    public GameManager GameManager { get; private set; }

    private AnimatedSprite coinSprite;
    private Label labelCoins;
    private int coins;
    private Camera2D camera;

    public void PreInit(GameManager gameManager)
    {
        GameManager = gameManager;
        
        var player = GetNode<Player>("Player");
        player.PreInit(this);

        //foreach (IEnemy child in GetNode<Node2D>("Environment/Enemies").GetChildren()) 
        //    child.PreInit(player);
    }

    public override void _Ready()
    {
        camera = GetNode<Camera2D>("Camera");
        labelCoins = GetNode<Label>(NodePathLabelCoins);
        coinSprite = GetNode<AnimatedSprite>(NodePathCoinSprite);
        coinSprite.Playing = true;

        CreateLevelBounds();
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
            new Vector2((Mathf.Abs(camera.LimitLeft) + Mathf.Abs(camera.LimitRight) / 2), colliderThickness)
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

    public void AddCoins(int amount = 1)
    {
        coins += amount;
        labelCoins.Text = "" + coins;
    }
}
