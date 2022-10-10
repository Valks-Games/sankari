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
		// Idle -> Walking
		// Idle -> Sprinting
		// Idle -> JumpStart
		// Idle -> JumpFall

		if (Player.IsOnGround())
		{
			if (Player.PlayerInput.IsJump)
				Transition(Player.AnimationJumpStart);

			if (Player.MoveDir != Vector2.Zero)
			{
				if (Player.PlayerInput.IsSprint)
					Transition(Player.AnimationRunning, 1.5f);
				else
					Transition(Player.AnimationWalking);
			}
		}
		else
		{
			if (Player.IsFalling())
				Transition(Player.AnimationJumpFall);
		}
	}
}
