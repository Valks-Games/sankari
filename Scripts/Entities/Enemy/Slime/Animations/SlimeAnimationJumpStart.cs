namespace Sankari;

public class SlimeAnimationJumpStart : EntityAnimation<Slime>
{
	private GTimer TimerDontCheckOnGround;

	public SlimeAnimationJumpStart(Slime entity) : base(entity) { }

	public override void EnterState()
	{
		TimerDontCheckOnGround = Entity.Timers.CreateTimer(100);
		TimerDontCheckOnGround.Loop = false;

		Entity.AnimatedSprite.Play("jump_start");
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
