using Sankari.Scripts.Player.Movement;

namespace Sankari;

public interface IMoveableEntity
{
	// Velocity of the entity
	Vector2 Velocity { get; set; }

	// Sprite to modify
	AnimatedSprite2D AnimatedSprite { get; }

	// Max speed on the ground while sprinting
	int SpeedMaxGroundSprint { get; }

	// Max speed on the ground while walking
	int SpeedMaxGround { get; }

	// Max speed when in the air
	int SpeedMaxAir { get; }

	// Current direction the entity is moving
	Vector2 MoveDir { get; }

	// Position in the world
	Vector2 GlobalPosition { get; }

	// Speed given when dashing vertically
	int SpeedDashVertical { get; }

	// Speed given when dashing horizontally
	int SpeedDashHorizontal { get; }

	// Horizontal wall jump force
	int JumpForceWallHorz { get; }

	// Vertical wall jump force
	int JumpForceWallVert { get; }

	// Is entity within wall jump-able area
	bool InWallJumpArea { get; }

	// Connection to the game world
	Window Tree { get; }

	// Wall direction
	int WallDir { get; }

	// Timers object that can be used to make timers
	GTimers Timers { get; }

	// Is the entity falling?
	bool IsFalling();

	// Is the entity on the ground?

	bool IsOnGround();

	// Force the entity to jump
	void Jump();
}

public abstract class PlayerCommand
{
	protected IMoveableEntity Entity { get; set; }

	public PlayerCommand(IMoveableEntity entity)
	{
		Entity = entity;
	}

	/// <summary>
	/// Called after parent enitity is ready
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
