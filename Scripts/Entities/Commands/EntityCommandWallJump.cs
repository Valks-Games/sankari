﻿namespace Sankari;

public interface IEntityWallJumpable : IEntityMoveable
{
	// Left wall checks
	public List<RayCast2D> RaycastsWallLeft { get; }

	// Right wall checks
	public List<RayCast2D> RaycastsWallRight { get; }

	// Is entity within wall jump-able area
	public bool InWallJumpArea { get; }

	// Max speed due to gravity
	public int ModGravityMaxSpeed { get; set; }

	// Is the entity falling?
	public bool IsFalling();
}

public class EntityCommandWallJump : EntityCommand<IEntityWallJumpable>
{
	#region Configuration

	// Horizontal wall jump force
	public int JumpForceWallHorz { get; set; } = 800;

	// Vertical wall jump force
	public int JumpForceWallVert { get; set; } = 500;

	// Sliding force downwards
	public int MaxGravitySpeedSliding { get; set; } = 20;

	// Fast sliding force downwards
	public int MaxGravitySpeedSlidingFast { get; set; } = 220;

	#endregion

	public event EventHandler WallJump;
	private int previousWallOnJump;
	private bool wasSliding = false;
	private float previousXDir;
	private int wallDir;

	public EntityCommandWallJump(IEntityWallJumpable entity) : base(entity) { }

	public override void Start()
	{
		if (Entity.InWallJumpArea)
		{
			// If the entity is on a wall, prevent entity from wall jumping on the same wall twice
			if (wallDir != 0)
			{
				// wall jump
				WallJump?.Invoke(this, EventArgs.Empty);

				Entity.AnimatedSprite.FlipH = wallDir == 1; // flip sprite on wall jump

				var velocity = Entity.Velocity;
				velocity.x += -JumpForceWallHorz * wallDir;
				velocity.y = -JumpForceWallVert;
				if (previousWallOnJump == wallDir)
				{
					velocity.y /= 2;
				}
				Entity.Velocity = velocity;

				previousWallOnJump = wallDir;
			}
		}
		else
			Entity.AnimatedSprite.FlipH = false;
	}

	public override void Update(float delta)
	{
		wallDir = UpdateWallDirection();
		if (Entity.IsNearGround())
		{
			previousWallOnJump = 0;
			wasSliding = false;
			return;
		}

		var isSliding = IsSliding();

		if (isSliding)
		{
			var velocity = Entity.Velocity;
			// Snap to nearest wall if we weren't sliding

			// Commenting this out fixes the "teleport issue" where the player teleports
			// small random distances whilst touching walls
			// Not sure why this was needed in the first place but it looks totally fine without it
			/*if (!wasSliding)
			{
				var collider = GetCollidingWall();

				if (collider != default)
				{
					Entity.GlobalPosition = new Vector2(collider.GetCollisionPoint().x, Entity.GlobalPosition.y);
				}
			}*/

			if (Entity.IsFalling())
			{
				Entity.ModGravityMaxSpeed = MaxGravitySpeedSliding;

				// fast fall
				if (Entity is Player player)
					if (player.PlayerInput.IsDown)
						Entity.ModGravityMaxSpeed = MaxGravitySpeedSlidingFast;

				// Slow down the player. (Faking Friction)
				if (velocity.y != Entity.ModGravityMaxSpeed)
					velocity = velocity.MoveToward(new Vector2(velocity.x, Entity.ModGravityMaxSpeed), 2000 * delta);
			}
			Entity.Velocity = velocity;
		}

		wasSliding = isSliding;
	}

	/// <summary>
	/// Gets the raycast which is colliding
	/// </summary>
	private RayCast2D GetCollidingWall()
	{
		if (wallDir == Vector2.Left.x)
			return Entity.RaycastsWallLeft.GetAnyRayCastCollider();

		else if (wallDir == Vector2.Right.x)
			return Entity.RaycastsWallRight.GetAnyRayCastCollider();

		return default;
	}

	/// <summary>
	/// Checks if the Entity should be sliding
	/// </summary>
	private bool IsSliding()
	{
		var velocityDir = MovementUtils.GetDirection(Entity.Velocity);

		// MoveDir takes priority
		if (Entity.MoveDir.x != 0)
			previousXDir = Entity.MoveDir.x;
		else if (velocityDir.x != 0)
			previousXDir = velocityDir.x;

		var isSliding = wasSliding;

		if (Entity.InWallJumpArea && wallDir != 0)
		{
			if (previousXDir == wallDir)
				isSliding = true;
			else if (previousXDir != 0)
				isSliding = false;
		}
		else
			isSliding = false;

		return isSliding;
	}

	private int UpdateWallDirection()
	{
		var left = Entity.RaycastsWallLeft.IsAnyRayCastColliding();
		var right = Entity.RaycastsWallRight.IsAnyRayCastColliding();

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}
}
