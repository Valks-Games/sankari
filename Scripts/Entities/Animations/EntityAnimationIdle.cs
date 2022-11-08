namespace Sankari;

public class EntityAnimationIdle<T> : EntityAnimation<T> where T : MovingEntity
{
	public EntityAnimationIdle(T entity) : base(entity) { }

	public override void Enter() => Entity.AnimatedSprite.Play("idle");

	/// <summary>
	/// <br>Idle -> JumpStart</br>
	/// <br>Idle -> JumpFall</br>
	/// </summary>
	public override void HandleTransitions()
	{
		if (!Entity.IsNearGround() && Entity.IsFalling())
			HandleTransitionsFalling();
		else if (!Entity.IsNearGround() && Entity.Velocity.y != 0)
			HandleTransitionsNearGround();
	}

	public virtual void HandleTransitionsFalling() =>
		SwitchState(EntityAnimationType.JumpFall);

	public virtual void HandleTransitionsNearGround() =>
		SwitchState(EntityAnimationType.JumpStart);
}
