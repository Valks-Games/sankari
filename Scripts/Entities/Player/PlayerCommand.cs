namespace Sankari;

public abstract class PlayerCommand : EntityCommand<Player>
{
	protected PlayerCommand(Player entity) : base(entity) { }

	public virtual void UpdateGroundWalking(float delta) { }

	public virtual void UpdateGroundSprinting(float delta) { }

	public virtual void UpdateAir(float delta) { }

	public virtual void Jump() { }

	public virtual void TouchedEnemy() { }

	public virtual void FinishedLevel() { }

	public virtual void Died() { }
}
