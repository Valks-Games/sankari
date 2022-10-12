namespace Sankari;

public class EntityAnimationDash<T> : EntityAnimation<T> where T : IEntityAnimation
{
	public EntityAnimationDash(T entity) : base(entity) { }

	protected override void EnterState()
	{
		// no animation for dash exists at this time
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// Dash -> Idle
		// Dash -> JumpFall
		// Dash -> Walking
		// Dash -> Running

		if (!Entity.CurrentlyDashing)
			if (!Entity.IsOnGround())
				if (Entity.Velocity.y > 0)
					SwitchState(Entity.AnimationJumpFall);
			else
				if (Entity.MoveDir != Vector2.Zero)
					if (Entity.PlayerInput.IsSprint)
						SwitchState(Entity.AnimationRunning);
					else
						SwitchState(Entity.AnimationWalking);
				else
					SwitchState(Entity.AnimationIdle);
	}

	protected override void ExitState()
	{

	}
}
