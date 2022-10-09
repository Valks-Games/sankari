namespace Sankari;

public abstract class PlayerCommand : EntityCommand
{
	public Player Player { get; set; }

	public PlayerCommand(Player player) => Player = player;

	public virtual void UpdateGroundWalking(float delta) { }

	public virtual void UpdateGroundSprinting(float delta) { }

	public virtual void UpdateAir(float delta) { }

	public virtual void Jump() { }

	public virtual void TouchedEnemy() { }

	public virtual void FinishedLevel() { }

	public virtual void Died() { }
}
