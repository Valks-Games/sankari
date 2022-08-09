namespace MarioLikeGame;

public class LevelScene : Node
{
    private Node _nodeEnemies;

    public void PreInit(GameManager gameManager)
    {
        GetNode<PlayerStateManager>("Player").PreInit(gameManager);

        /*// testing
        _nodeEnemies = GetNode<Node>("Enemies");
        foreach (Node2D enemy in _nodeEnemies.GetChildren())
        {
            var position = enemy.Position;
        }

        //
        var coins = GetNode<Node>("Coins");
        foreach (AnimatedSprite coin in coins.GetChildren())
        {

        }*/
    }
}
