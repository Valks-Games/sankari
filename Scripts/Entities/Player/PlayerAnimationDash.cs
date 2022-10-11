using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationDash : PlayerAnimation
{
	public PlayerAnimationDash(Player player) : base(player) { }

	protected override void EnterState()
	{
		// no animation for dash exists at this time
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
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
					SwitchState(Player.AnimationJumpFall);
			}
			else
			{
				if (Player.MoveDir != Vector2.Zero)
				{
					if (Player.PlayerInput.IsSprint)
						SwitchState(Player.AnimationRunning);
					else
						SwitchState(Player.AnimationWalking);
				}
				else
				{
					SwitchState(Player.AnimationIdle);
				}
			}
		}
	}

	protected override void ExitState()
	{
		
	}
}
