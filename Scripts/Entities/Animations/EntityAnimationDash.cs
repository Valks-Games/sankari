namespace Sankari;

public interface IEntityAnimationDash : IEntityDash, IEntityAnimation
{
}

public class EntityAnimationDash : EntityAnimation<IEntityAnimationDash>
{
	public EntityAnimationDash(IEntityAnimationDash entity) : base(entity)
	{
	}

	public override void EnterState()
	{
		// no animation for dash exists at this time
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// Dash -> Idle Dash -> JumpFall Dash -> Walking Dash -> Running

		if (!Entity.CurrentlyDashing)
			if (!Entity.IsOnGround())
				if (Entity.Velocity.y > 0)
					SwitchState(Entity.AnimationJumpFall);
				else
				if (Entity.MoveDir != Vector2.Zero)
					if (Entity is Player player && player.PlayerInput.IsSprint)
						SwitchState(Entity.AnimationRunning);
					else
						SwitchState(Entity.AnimationWalking);
				else
					SwitchState(Entity.AnimationIdle);
	}

	public override void ExitState()
	{
	}
}
