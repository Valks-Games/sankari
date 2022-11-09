namespace Sankari;

public class EntityAnimationJumpStart<T> : EntityAnimation<T> where T : MovingEntity<T>
{
	private GTimer TimerDontCheckOnGround { get; set; }

	public EntityAnimationJumpStart(T entity) : base(entity) { }

	public override void Enter()
	{
		TimerDontCheckOnGround = Entity.Timers.CreateTimer(100);
		TimerDontCheckOnGround.Loop = false;
		
		Entity.AnimatedSprite.Play("jump_start");
	}

	/// <summary>
	/// <br>JumpStart -> Idle</br>
	///	<br>JumpStart -> JumpFall</br>
	/// </summary>
	public override void HandleTransitions()
	{
		if (Entity.IsFalling())
			HandleTransitionsFalling();
		else if (Entity.IsNearGround() && Entity.MoveDir == Vector2.Zero && !TimerDontCheckOnGround.IsActive())
			HandleTransitionsNearGround();
	}

	public virtual void HandleTransitionsFalling() =>
		SwitchState(EntityAnimationType.JumpFall);

	public virtual void HandleTransitionsNearGround() =>
		SwitchState(EntityAnimationType.Idle);
}
