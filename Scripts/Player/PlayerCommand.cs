namespace Sankari;

public abstract class PlayerCommand
{
	public virtual void Init(Player player) { }
	public virtual void Update(Player player) { }
}
