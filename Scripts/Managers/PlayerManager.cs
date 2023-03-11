namespace Sankari;
public class PlayerManager
{
    public int PlayerHealth { get; set; }
    private int PlayerHealthMax { get; set; } = 20;

    //The number of additional lives (i.e. the player can die Lives+1 times)
    public int Lives { get; private set; }
    private int LivesMax { get; set; } = 10;

    public int Coins { get; private set; }
    private int CheckpointCoins { get; set; }
    private int LevelCoins { get; set; }
    public Vector2 RespawnPosition { get; set; }
    public bool ActiveCheckpoint { get; set; } = false;


    public PlayerManager(int lives, int health, int coins = 0)
    {
        Lives = lives;
        PlayerHealth = health;
        Coins = coins;
    }


    public void SetLives(int lives) => Lives = lives;
    public void AddLife()
    {
        if(Lives < LivesMax)
            Lives++;
    }
    public bool RemoveLife()
    {
        SetCoins(CheckpointCoins);
        if (Lives-- > 0)
            return true;
        else
        {
            ActiveCheckpoint = false;
            return false;
        }
    }


    public void ResetCoins() => Coins = LevelCoins;
    public void SetCoins(int coins) => Coins = coins;
    public void AddCoins(int amount = 1) => Coins += amount;
    public bool RemoveCoin(int amount = 0, bool toZero = false)
    {
        if(!toZero)
        {
            if ((Coins -= amount) >= 0)
                return true;
            else
                return false;
        }
        else
        {
            Coins = 0;
            return true;
        }
    }
    public void SetCheckpointCoins() => CheckpointCoins = Coins;
    public void SetLevelCoins() => LevelCoins = Coins;
}