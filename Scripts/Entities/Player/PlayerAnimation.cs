namespace Sankari;

public abstract class PlayerAnimation
{
	protected Player Player { get; set; }

	protected PlayerAnimation(Player player) { Player = player; }

	protected abstract void EnterState();
	public abstract void UpdateState();
	protected abstract void ExitState();
	
	protected void SwitchState(PlayerAnimation animation)
	{
		Player.CurrentAnimation.ExitState();
		Player.CurrentAnimation = animation;
		Player.CurrentAnimation.EnterState();
	}

	protected void FlipSpriteOnDirection() =>
		Player.AnimatedSprite.FlipH = Player.MoveDir.x < 0; // flip sprite if moving left

	public override string ToString() => GetType().Name.Replace(nameof(PlayerAnimation), "");
}
