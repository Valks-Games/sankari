namespace Sankari;

public class LevelScene : Node
{
    public void PreInit(GameManager gameManager)
    {
        var player = GetNode<Player>("Player");
        player.PreInit(gameManager);

        foreach (IEnemy child in GetNode<Node2D>("Environment/Enemies").GetChildren()) 
            child.PreInit(player);
    }
}
