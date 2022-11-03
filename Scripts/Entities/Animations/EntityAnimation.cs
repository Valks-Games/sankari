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

public abstract class EntityAnimation<T> : EntityAnimation where T : MovingEntity
{
	protected T Entity { get; set; }

	public EntityAnimation(T entity) : base()
	{
		Entity = entity;
	}

	protected void SwitchState(EntityAnimationType animation)
	{
		if (!Entity.Animations.ContainsKey(animation))
		{
			Logger.LogWarning($"{animation} for {Entity} has not been setup yet");
			return;
		}

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
	public EntityAnimation() { }

	/// <summary>
	/// Setup and start the animation
	/// </summary>
	public abstract void EnterState();

	/// <summary>
	/// Update the current animation
	/// </summary>
	public abstract void UpdateState();

	/// <summary>
	/// Potentially transition to a new state
	/// </summary>
	public abstract void HandleStateTransitions();

	/// <summary>
	/// Cleanly exit the animation state
	/// </summary>
	public abstract void ExitState();
}
