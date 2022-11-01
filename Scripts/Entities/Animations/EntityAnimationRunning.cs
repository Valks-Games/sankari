namespace Sankari;

public class EntityAnimationRunning : EntityAnimation<IEntityAnimation>
{
	public EntityAnimationRunning(IEntityAnimation entity) : base(entity)
	{
	}

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("walk");
		Entity.AnimatedSprite.SpeedScale = 1.5f;
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// Running -> Idle
		// Running -> Walking
		// Running -> Dash
		// Running -> JumpStart

		if (Entity is Player player)
		{
			if (player.PlayerInput.IsJump)

				SwitchState(EntityAnimationType.JumpStart);

			else if (player.PlayerInput.IsDash && player.GetCommandClass<EntityCommandDash>(EntityCommandType.Dash).DashReady)

				SwitchState(EntityAnimationType.Dash);

			else if (!player.PlayerInput.IsSprint)

				SwitchState(EntityAnimationType.Walking);

			else if (Entity.MoveDir == Vector2.Zero || (player.Velocity.y != 0))

				SwitchState(EntityAnimationType.Idle);
		}
	}

	public override void ExitState()
	{
		Entity.AnimatedSprite.SpeedScale = 1.0f;
	}
}
