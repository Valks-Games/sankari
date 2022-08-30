namespace Sankari;

public class LevelScene : Node
{
    [Export] protected readonly NodePath NodePathCoinSprite;
    [Export] protected readonly NodePath NodePathLabelCoins;

    public GameManager GameManager { get; private set; }

    private AnimatedSprite coinSprite;
    private Label labelCoins;
    private int coins;

    public void PreInit(GameManager gameManager)
    {
        GameManager = gameManager;
        
        var player = GetNode<Player>("Player");
        player.PreInit(this);

        foreach (IEnemy child in GetNode<Node2D>("Environment/Enemies").GetChildren()) 
            child.PreInit(player);
    }

    public override void _Ready()
    {
        labelCoins = GetNode<Label>(NodePathLabelCoins);
        coinSprite = GetNode<AnimatedSprite>(NodePathCoinSprite);
        coinSprite.Playing = true;
    }

    public void AddCoins(int amount = 1)
    {
        coins += amount;
        labelCoins.Text = "" + coins;
    }
}
