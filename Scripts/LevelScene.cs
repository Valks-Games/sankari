namespace MarioLikeGame;

public class LevelScene : Node
{
    public void PreInit(GameManager gameManager)
    {
        var player = GetNode<PlayerStateManager>("Player");
        player.PreInit(gameManager);

        foreach (IEnemy child in GetNode<Node2D>("Enemies").GetChildren()) 
            child.PreInit(player);
    }
}
