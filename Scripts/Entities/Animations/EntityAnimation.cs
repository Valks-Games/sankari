namespace Sankari;

public interface IEntityAnimation : IEntityBase
{
	public EntityAnimation          CurrentAnimation   { get; set; }
	public EntityAnimationIdle      AnimationIdle      { get; set; }
	public EntityAnimationWalking   AnimationWalking   { get; set; }
	public EntityAnimationRunning   AnimationRunning   { get; set; }
	public EntityAnimationJumpStart AnimationJumpStart { get; set; }
	public EntityAnimationJumpFall  AnimationJumpFall  { get; set; }
	public EntityAnimationDash      AnimationDash      { get; set; }
	public AnimatedSprite2D         AnimatedSprite     { get; set; }
}

public abstract class EntityAnimation<T> : EntityAnimation where T : IEntityAnimation
{
	protected T Entity { get; set; }

	public EntityAnimation(T entity) : base()
	{
		Entity = entity;
	}

	protected void SwitchState(EntityAnimation animation)
	{
		Logger.Log(animation);
		Entity.CurrentAnimation.ExitState();
		Entity.CurrentAnimation = animation;
		Entity.CurrentAnimation.EnterState();
	}

	protected void FlipSpriteOnDirection() =>
		Entity.AnimatedSprite.FlipH = Entity.MoveDir.x < 0; // flip sprite if moving left

	public override string ToString() => GetType().Name.Replace(nameof(EntityAnimation), "");
}

public abstract class EntityAnimation
{
	public EntityAnimation()
	{ }

	public abstract void UpdateState();

	public abstract void HandleStateTransitions();

	public abstract void EnterState();

	public abstract void ExitState();
}
