namespace Sankari;

public interface IEntityMoveable
{
	// Velocity of the entity
	public Vector2 Velocity { get; set; }

	// Current direction the entity is moving
	public Vector2 MoveDir { get; }

	// Position in the world
	public Vector2 GlobalPosition { get; }

	// Connection to the game world
	public Window Tree { get; }

	// Timers object that can be used to make timers
	public GTimers Timers { get; }

}

public abstract class EntityCommand
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

	/// <summary>
	/// Called when there's a need to prematurely stop the Movement
	/// </summary>
	public virtual void Stop()
	{ }
}

public abstract class EntityCommand<T> : EntityCommand
{
	protected T Entity { get; set; }

	public EntityCommand(T entity)
	{
		Entity = entity;
	}

}
