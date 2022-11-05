namespace Sankari;

public interface IEntityHealthSystem
{
	public int EntityHealth { get; set; }
	public int EntityHealthMax { get; set; }

	public bool SetHealth(int amount);
	public bool AddHealth(int amount);
	public bool RemoveHealth(int amount);
}

public class EntityHealthSystem : IEntityHealthSystem
{
	public int EntityHealth { get; set; }
	public int EntityHealthMax { get; set; } = 20;

	public EntityHealthSystem(int entityHealth) => EntityHealth = entityHealth;

	public bool SetHealth(int amount)
	{
		if (amount < EntityHealthMax)
		{
			EntityHealth = amount;
			return true;
		}
		else
		{
			EntityHealth = EntityHealthMax;
			return false;
		}
	}
	public bool AddHealth(int amount)
	{
		if (EntityHealth + amount < EntityHealthMax)
		{
			EntityHealth += amount;
			return true;
		}
		else
		{
			EntityHealth = EntityHealthMax;
			return false;
		}
	}
	public bool RemoveHealth(int amount)
	{
		if ((EntityHealth -= amount) > 0)
			return true;
		else
		{
			EntityHealth = 0;
			return false;
		}
	}
}