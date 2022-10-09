namespace Sankari;

public abstract class PlayerCommand
{
	public Player Player { get; set; }

	public PlayerCommand(Player player) => Player = player;

	public virtual void Init() { }

	public virtual void Update(float delta) { }

	public virtual void UpdateGroundWalking() { }

	public virtual void UpdateGroundSprinting() { }

	public virtual void UpdateAir() { }

	public virtual void Jump() { }

	public virtual void TouchedEnemy() { }

	public virtual void FinishedLevel() { }

	public virtual void Died() { }
}
