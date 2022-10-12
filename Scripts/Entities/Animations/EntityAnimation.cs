namespace Sankari;

public interface IEntityAnimation
{
	public EntityAnimation<IEntityAnimation>          CurrentAnimation   { get; set; }
	public EntityAnimationIdle<IEntityAnimation>      AnimationIdle      { get; set; }
	public EntityAnimationWalking<IEntityAnimation>   AnimationWalking   { get; set; }
	public EntityAnimationRunning<IEntityAnimation>   AnimationRunning   { get; set; }
	public EntityAnimationJumpStart<IEntityAnimation> AnimationJumpStart { get; set; }
	public EntityAnimationJumpFall<IEntityAnimation>  AnimationJumpFall  { get; set; }
	public AnimatedSprite2D                           AnimatedSprite     { get; set; }
	public Vector2                                    MoveDir            { get; set; }
}

public abstract class EntityAnimation<T> where T : IEntityAnimation
{
	protected T Entity { get; set; }

	protected EntityAnimation(T entity) { Entity = entity; }

	public abstract void UpdateState();
	public abstract void HandleStateTransitions();

	protected abstract void EnterState();
	protected abstract void ExitState();

	protected void SwitchState(EntityAnimation<T> animation)
	{
		Logger.Log(animation);
		Entity.CurrentAnimation.ExitState();
		Entity.CurrentAnimation = animation;
		Entity.CurrentAnimation.EnterState();
	}

	protected void FlipSpriteOnDirection() =>
		Entity.AnimatedSprite.FlipH = Entity.MoveDir.x < 0; // flip sprite if moving left

	public override string ToString() => GetType().Name.Replace(nameof(EntityAnimation<T>), "");
}
