namespace Sankari;

public class EntityAnimationJumpFall : EntityAnimation<MovingEntity>
{
	public EntityAnimationJumpFall(MovingEntity entity) : base(entity) { }

	public override void EnterState()
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
		// JumpFall -> JumpStart

		if (Entity.IsNearGround())
			if (Entity.MoveDir != Vector2.Zero)
				if (Entity is Player player && player.PlayerInput.IsSprint)
					SwitchState(EntityAnimationType.Running);
				else
					SwitchState(EntityAnimationType.Walking);
			else
				SwitchState(EntityAnimationType.Idle);
		else if (Entity is Player player && player.PlayerInput.IsDash && Entity.GetCommandClass<MovingEntityCommandDash>(EntityCommandType.Dash).DashReady)
			SwitchState(EntityAnimationType.Dash);
		else if (!Entity.IsFalling() && Entity.Velocity.y != 0)
			SwitchState(EntityAnimationType.JumpStart);
	}

	public override void ExitState()
	{
		
	}
}
