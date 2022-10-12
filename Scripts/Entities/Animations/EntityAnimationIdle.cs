namespace Sankari;

public class EntityAnimationIdle<T> : EntityAnimation<T> where T : IEntityAnimation
{
	public EntityAnimationIdle(T entity) : base(entity) { }

	protected override void EnterState()
	{
		Entity.AnimatedSprite.Play("idle");
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
