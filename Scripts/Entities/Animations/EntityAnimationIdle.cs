namespace Sankari;

public class EntityAnimationIdle : EntityAnimation
{
	public EntityAnimationIdle(Player player) : base(player) { }

	protected override void EnterState()
	{
		Player.AnimatedSprite.Play("idle");
	}

	public override void UpdateState()
	{

	}

	public override void HandleStateTransitions()
	{
		// Idle -> Walking
		// Idle -> Sprinting
		// Idle -> JumpStart
		// Idle -> JumpFall

		if (Player.IsOnGround())
		{
			if (Player.PlayerInput.IsJump)
				SwitchState(Player.AnimationJumpStart);

			if (Player.MoveDir != Vector2.Zero)
				if (Player.PlayerInput.IsSprint)
					SwitchState(Player.AnimationRunning);
				else
					SwitchState(Player.AnimationWalking);
		}
		else
			if (Player.IsFalling())
				SwitchState(Player.AnimationJumpFall);
	}

	protected override void ExitState()
	{

	}
}
