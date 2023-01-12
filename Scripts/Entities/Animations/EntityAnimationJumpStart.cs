namespace Sankari;

public class EntityAnimationJumpStart<T> : EntityAnimation<T> where T : MovingEntity<T>
{
	private GTimer TimerDontCheckOnGround { get; set; }

	public EntityAnimationJumpStart(T entity) : base(entity) { }

	public override void Enter()
	{
		TimerDontCheckOnGround = Entity.Timers.CreateTimer(100);
		
		Entity.AnimatedSprite.Play("jump_start");
	}

	/// <summary>
	/// <br>JumpStart -> Idle</br>
	///	<br>JumpStart -> JumpFall</br>
	/// </summary>
	public override void HandleTransitions()
	{
		if (Entity.IsNearGround() && Entity.MoveDir == Vector2.Zero && !TimerDontCheckOnGround.IsActive())
			HandleTransitionsIdleNearGround();
		else if (Entity.IsNearGround() && !TimerDontCheckOnGround.IsActive())
			SwitchState(EntityAnimationType.Walking);
		else if (Entity.IsFalling())
			HandleTransitionsFalling();
	}

	public virtual void HandleTransitionsFalling() =>
		SwitchState(EntityAnimationType.JumpFall);

	public virtual void HandleTransitionsIdleNearGround() =>
		SwitchState(EntityAnimationType.Idle);
}
