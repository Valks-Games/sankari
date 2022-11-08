namespace Sankari;

public class EntityAnimationJumpStart : EntityAnimation<MovingEntity>
{
	private GTimer TimerDontCheckOnGround;

	public EntityAnimationJumpStart(MovingEntity entity) : base(entity) { }

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("jump_start");

		TimerDontCheckOnGround = Entity.Timers.CreateTimer(100);
		TimerDontCheckOnGround.Loop = false;
	}

	public override void ExitState()
	{
		
	}

	public override void HandleStateTransitions()
	{
		// JumpStart -> Idle
		// JumpStart -> JumpFall

		if (Entity.IsFalling())
			SwitchState(EntityAnimationType.JumpFall);
		else if (Entity.IsNearGround() && Entity.MoveDir == Vector2.Zero && !TimerDontCheckOnGround.IsActive())
			SwitchState(EntityAnimationType.Idle);
	}

	public override void UpdateState()
	{
		
	}
}
