using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationDash : PlayerAnimation
{
	public PlayerAnimationDash(Player player) : base(player) { }

	public override void EnterState()
	{
		// no animation for dash exists at this time
	}

	public override void UpdateState()
	{
		// Dash -> Idle
		// Dash -> JumpFall
		// Dash -> Walking
		// Dash -> Running

		if (!Player.CurrentlyDashing)
		{
			if (!Player.IsOnGround())
			{
				if (Player.Velocity.y > 0)
					Transition(Player.AnimationJumpFall);
			}
			else
			{
				if (Player.MoveDir != Vector2.Zero)
				{
					if (Player.PlayerInput.IsSprint)
						Transition(Player.AnimationRunning, 1.5f);
					else
						Transition(Player.AnimationWalking);
				}
				else
				{
					Transition(Player.AnimationIdle);
				}
			}
		}	
	}
}
