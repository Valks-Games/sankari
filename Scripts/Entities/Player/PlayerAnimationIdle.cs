using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationIdle : PlayerAnimation
{
	public PlayerAnimationIdle(Player player) : base(player) { }

	public override void EnterState()
	{
		Player.AnimatedSprite.Play("idle");
	}

	public override void UpdateState()
	{
		// Idle -> JumpStart
		// Idle -> Walking
		// Idle -> Sprinting

		if (Player.PlayerInput.IsJump)
		{
			Transition(Player.AnimationJumpStart);
		}

		if (Player.MoveDir != Vector2.Zero)
		{
			if (Player.PlayerInput.IsSprint)
			{
				Transition(Player.AnimationRunning, 1.5f);
			}
			else
			{
				Transition(Player.AnimationWalking);
			}
		}
	}
}
