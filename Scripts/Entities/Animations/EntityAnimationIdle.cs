namespace Sankari;

public class EntityAnimationIdle : EntityAnimation<IEntityAnimation>
{
	public EntityAnimationIdle(IEntityAnimation entity) : base(entity)
	{
	}

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("idle");
		// Idle animation should only occur if nothing else interesting is happening
		// Transition immediately to see if there is a better animation to be in
		HandleStateTransitions();
	}

	public override void HandleStateTransitions()
	{
		// Idle -> Walking
		// Idle -> Sprinting
		// Idle -> JumpStart
		// Idle -> JumpFall

		if (Entity is Player player)
		{
			if (player.IsNearGround())
			{
				if (player.PlayerInput.IsJump)
					SwitchState(EntityAnimationType.JumpStart);

				if (Entity.MoveDir != Vector2.Zero)
					if (player.PlayerInput.IsSprint)
						SwitchState(EntityAnimationType.Running);
					else
						SwitchState(EntityAnimationType.Walking);
			}
			else if (player.IsFalling())
				SwitchState(EntityAnimationType.JumpFall);
			else if (player.Velocity.y != 0)
				SwitchState(EntityAnimationType.JumpStart);
		}
	}
}
