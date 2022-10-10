using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationJumpFall : PlayerAnimation
{
	public PlayerAnimationJumpFall(Player player) : base(player) { }

	public override void EnterState()
	{
		Player.AnimatedSprite.Play("jump_fall");
	}

	public override void UpdateState()
	{
		// JumpFall -> Idle
		// JumpFall -> Walking
		// JumpFall -> Running
		// JumpFall -> Dash

		if (Player.IsOnGround())
		{
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
			else
			{
				Transition(Player.AnimationIdle);
			}
		}
		else if (Player.PlayerInput.IsDash)
		{
			Transition(Player.AnimationDash);
		}
	}
}
