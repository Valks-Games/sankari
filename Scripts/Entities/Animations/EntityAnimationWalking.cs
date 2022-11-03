namespace Sankari;

public class EntityAnimationWalking : EntityAnimation<Entity>
{
	public EntityAnimationWalking(Entity entity) : base(entity) { }

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
				SwitchState(EntityAnimationType.JumpStart);

			else if (player.PlayerInput.IsDash && player.GetCommandClass<EntityCommandDash>(EntityCommandType.Dash).DashReady)
				SwitchState(EntityAnimationType.Dash);

			else if (player.PlayerInput.IsSprint)
				SwitchState(EntityAnimationType.Running);

			else if (player.MoveDir == Vector2.Zero || player.Velocity.y != 0)
				SwitchState(EntityAnimationType.Idle);
		}
	}

	public override void ExitState()
	{
		
	}
}
