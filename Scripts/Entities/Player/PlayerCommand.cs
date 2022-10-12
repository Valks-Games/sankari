namespace Sankari;

// unused class?
public abstract class PlayerCommand : EntityCommand<Player>
{
	protected PlayerCommand(Player entity) : base(entity) { }

	public virtual void Jump() { }

	public virtual void TouchedEnemy() { }

	public virtual void FinishedLevel() { }

	public virtual void Died() { }
}
