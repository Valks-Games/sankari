namespace Sankari;

public class EntityAnimationWalking<T> : EntityAnimation<T> where T : IEntityAnimation
{
	public EntityAnimationWalking(T entity) : base(entity) { }

	protected override void EnterState()
	{
		Entity.AnimatedSprite.Play("walk");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// Walking -> Idle
		// Walking -> Running
		// Walking -> Dash
		// Walking -> JumpStart

		if (Entity.PlayerInput.IsJump)

			SwitchState(Entity.AnimationJumpStart);

		else if (Entity.PlayerInput.IsDash)

			SwitchState(Entity.AnimationDash);

		else if (Entity.PlayerInput.IsSprint)

			SwitchState(Entity.AnimationRunning);

		else if (Entity.MoveDir == Vector2.Zero)

			SwitchState(Entity.AnimationIdle);
	}

	protected override void ExitState()
	{

	}
}
