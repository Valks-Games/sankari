namespace Sankari;

public interface IEntityMoveable
{
	// Velocity of the entity
	Vector2 Velocity { get; set; }

	// Current direction the entity is moving
	Vector2 MoveDir { get; }

	// Position in the world
	Vector2 GlobalPosition { get; }

	// Connection to the game world
	Window Tree { get; }

	// Timers object that can be used to make timers
	GTimers Timers { get; }

}

public abstract class PlayerCommand
{
	/// <summary>
	/// Called after parent entity is ready
	/// </summary>
	public virtual void Initialize()
	{ }

	/// <summary>
	/// Called when Movement occurs.
	/// </summary>
	/// <param name="input">Input to act on</param>
	public virtual void Update(MovementInput input)
	{ }

	/// <summary>
	/// Called after most Movement logic occurs
	/// </summary>
	/// <param name="input">Input to act on</param>
	public virtual void LateUpdate(MovementInput input)
	{ }

}

public abstract class EntityCommand<T> : PlayerCommand
{
	protected T Entity { get; set; }

	public EntityCommand(T entity)
	{
		Entity = entity;
	}

}
