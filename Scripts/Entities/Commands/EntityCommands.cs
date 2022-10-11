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

	// Whether gravity is enabled or not
	public bool GravityEnabled { get; set; }

	// Assuming all movable entites have a animated sprite
	public AnimatedSprite2D AnimatedSprite { get; set; }

	// Checks if the entity is on the ground
	public bool IsOnGround();
}

public abstract class EntityCommand
{
	/// <summary>
	/// Called after parent entity is ready
	/// </summary>
	public virtual void Initialize() {}

	/// <summary>
	/// Called when Movement occurs.
	/// </summary>
	/// <param name="input">Input to act on</param>
	public virtual void Update(float delta) {}

	/// <summary>
	/// Called when moving in the air
	/// </summary>
	/// <param name="delta"></param>
	public virtual void UpdateAir(float delta) {}

	/// <summary>
	/// Called after most Movement logic occurs
	/// </summary>
	/// <param name="input">Input to act on</param>
	public virtual void LateUpdate(float delta) {}

	/// <summary>
	/// Start the command
	/// </summary>
	public virtual void Start() {}
	
	/// <summary>
	/// Stop the command
	/// </summary>
	public virtual void Stop() {}
}

public abstract class EntityCommand<T> : EntityCommand
{
	protected T Entity { get; set; }

	public EntityCommand(T entity)
	{
		Entity = entity;
	}
}
