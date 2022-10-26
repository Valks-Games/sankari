namespace Sankari;

public enum EntityAnimationType
{
	None = 0,
	Idle,
	Walking,
	Running,
	JumpStart,
	JumpFall,
	Dash
}

public interface IEntityAnimation : IEntityBase
{
	public Dictionary<EntityAnimationType, EntityAnimation> Animations   { get; }
	public Dictionary<EntityCommandType, EntityCommand>     Commands     { get; }
	public EntityAnimationType                              CurrentAnimation   { get; set; }
	public AnimatedSprite2D                                 AnimatedSprite     { get; }
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

	/// <summary>
	/// Update the current animation
	/// </summary>
	public virtual void UpdateState()
	{ }

	/// <summary>
	/// Potentially transition to a new state
	/// </summary>
	public virtual void HandleStateTransitions()
	{ }

	/// <summary>
	/// Setup and start the animation
	/// </summary>
	public virtual void EnterState()
	{ }

	/// <summary>
	/// Cleanly exit the animation state
	/// </summary>
	public virtual void ExitState()
	{ }
}
