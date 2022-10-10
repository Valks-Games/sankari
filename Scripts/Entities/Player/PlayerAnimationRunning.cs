using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationRunning : PlayerAnimation
{
	public PlayerAnimationRunning(Player player) : base(player) { }

	public override void EnterState()
	{
		Player.AnimatedSprite.Play("walk");
	}

	public override void UpdateState()
	{
		// Running -> Idle
		// Running -> Walking
		// Running -> Dash
		// Running -> JumpStart

		if (Player.PlayerInput.IsJump)
		{
			Transition(Player.AnimationJumpStart);
		}
		else if (Player.PlayerInput.IsDash)
		{
			Transition(Player.AnimationDash);
		}
		else if (!Player.PlayerInput.IsSprint)
		{
			Transition(Player.AnimationWalking);
		}
		else if (Player.MoveDir == Vector2.Zero)
		{
			Transition(Player.AnimationIdle);
		}
	}
}
