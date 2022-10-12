namespace Sankari;

public class EntityAnimationJumpFall<T> : EntityAnimation<T> where T : IEntityAnimation
{
	public EntityAnimationJumpFall(T entity) : base(entity) { }

	protected override void EnterState()
	{
		Entity.AnimatedSprite.Play("jump_fall");
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

		if (Entity.IsOnGround())
			if (Entity.MoveDir != Vector2.Zero)
				if (Entity.PlayerInput.IsSprint)
					SwitchState(Entity.AnimationRunning);
				else
					SwitchState(Entity.AnimationWalking);
			else
				SwitchState(Entity.AnimationIdle);
		else if (Entity.PlayerInput.IsDash)
			SwitchState(Entity.AnimationDash);
	}

	protected override void ExitState()
	{

	}
}
