namespace Sankari;

public enum EntityAnimationType
{
	Idle,
	Walking,
	Running,
	JumpStart,
	JumpFall,
	Dash
}

public interface IEntityAnimation : IEntityBase
{
	public Dictionary<EntityAnimationType, EntityAnimation> Animations   { get; set; }
	public EntityAnimationType                              CurrentAnimation   { get; set; }
	public AnimatedSprite2D                                 AnimatedSprite     { get; set; }
}

public abstract class EntityAnimation<T> : EntityAnimation where T : IEntityAnimation
{
	protected T Entity { get; set; }

	public EntityAnimation(T entity) : base()
	{
		Entity = entity;
	}

	protected void SwitchState(EntityAnimationType animation)
	{
		Logger.Log(animation);
		Entity.Animations[Entity.CurrentAnimation].ExitState();
		Entity.CurrentAnimation = animation;
		Entity.Animations[Entity.CurrentAnimation].EnterState();
	}

	protected void FlipSpriteOnDirection()
	{
		if (Entity.MoveDir.x != 0)
		{
			Entity.AnimatedSprite.FlipH = Entity.MoveDir.x < 0; // flip sprite if moving left
		}
	}

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
