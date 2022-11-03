namespace Sankari;

public class EntityAnimationJumpFall : EntityAnimation<Entity>
{
	public EntityAnimationJumpFall(Entity entity) : base(entity) { }

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

		if (Entity is Player player)
		{
			if (player.IsNearGround())
				if (Entity.MoveDir != Vector2.Zero)
					if (player.PlayerInput.IsSprint)
						SwitchState(EntityAnimationType.Running);
					else
						SwitchState(EntityAnimationType.Walking);
				else
					SwitchState(EntityAnimationType.Idle);
			else if (player.PlayerInput.IsDash && player.GetCommandClass<EntityCommandDash>(EntityCommandType.Dash).DashReady)
				SwitchState(EntityAnimationType.Dash);
			else if (!player.IsFalling() && player.Velocity.y != 0)
				SwitchState(EntityAnimationType.JumpStart);
		}
	}

	public override void ExitState()
	{
		
	}
}
