namespace Sankari;
public class PlayerManager
{
	public int PlayerHealth { get; set; }
	public int PlayerHealthMax { get; set; } = 20;
	//The number of additional lives (i.e. the player can die Lives+1 times)
	public int Lives { get; set; }
	public int LivesMax { get; set; } = 10;


	public PlayerManager(int lives, int health)
	{
		Lives = lives;
		PlayerHealth = health;
	}

	public void SetLives(int lives)
	{
		Lives = lives;
	}
	public void AddLife()
	{
		if(Lives<LivesMax)
			Lives++;
	}
	public bool RemoveLife()
	{
		if(Lives-->0)
			return true;
		else
			return false;
	}
}