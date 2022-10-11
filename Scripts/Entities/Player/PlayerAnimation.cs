namespace Sankari;

public abstract class PlayerAnimation
{
	protected Player Player { get; set; }

	protected PlayerAnimation(Player player) { Player = player; }

	public abstract void UpdateState();
	public abstract void HandleStateTransitions();

	protected abstract void EnterState();
	protected abstract void ExitState();
	
	protected void SwitchState(PlayerAnimation animation)
	{
		Logger.Log(animation);
		Player.CurrentAnimation.ExitState();
		Player.CurrentAnimation = animation;
		Player.CurrentAnimation.EnterState();
	}

	protected void FlipSpriteOnDirection() =>
		Player.AnimatedSprite.FlipH = Player.MoveDir.x < 0; // flip sprite if moving left

	public override string ToString() => GetType().Name.Replace(nameof(PlayerAnimation), "");
}
