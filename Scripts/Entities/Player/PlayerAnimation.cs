namespace Sankari;

public abstract class PlayerAnimation
{
	protected Player Player { get; set; }

	protected PlayerAnimation(Player player) { Player = player; }

	public abstract void EnterState();
	public abstract void UpdateState();
	
	protected void Transition(PlayerAnimation animation, float animationSpeed = 1.0f)
	{
		Player.CurrentAnimation = animation;
		Player.CurrentAnimation.EnterState();
		Player.AnimatedSprite.SpeedScale = animationSpeed;
	}

	public override string ToString() => GetType().Name.Replace(nameof(PlayerAnimation), "");
}
