namespace Sankari;

public class EntityAnimationJumpFall : EntityAnimation
{
	public EntityAnimationJumpFall(Player player) : base(player) { }

	protected override void EnterState()
	{
		Player.AnimatedSprite.Play("jump_fall");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// JumpFall -> Idle
		// JumpFall -> Walking
		// JumpFall -> Running
		// JumpFall -> Dash

		if (Player.IsOnGround())
			if (Player.MoveDir != Vector2.Zero)
				if (Player.PlayerInput.IsSprint)
					SwitchState(Player.AnimationRunning);
				else
					SwitchState(Player.AnimationWalking);
			else
				SwitchState(Player.AnimationIdle);
		else if (Player.PlayerInput.IsDash)
			SwitchState(Player.AnimationDash);
	}

	protected override void ExitState()
	{

	}
}
