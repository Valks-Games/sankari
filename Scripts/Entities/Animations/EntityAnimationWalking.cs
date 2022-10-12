namespace Sankari;

public class EntityAnimationWalking : EntityAnimation<IEntityAnimation>
{
	public EntityAnimationWalking(IEntityAnimation entity) : base(entity)
	{
	}

	public override void EnterState()
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

		if (Entity is Player player)
		{
			if (player.PlayerInput.IsJump)

				SwitchState(Entity.AnimationJumpStart);
			else if (player.PlayerInput.IsDash)

				SwitchState(Entity.AnimationDash);
			else if (player.PlayerInput.IsSprint)

				SwitchState(Entity.AnimationRunning);
			else if (player.MoveDir == Vector2.Zero)

				SwitchState(Entity.AnimationIdle);
		}
	}

	public override void ExitState()
	{
	}
}
