﻿namespace Sankari;

public interface IEntityWallJumpable : IEntityMoveable
{
	// Left wall checks
	public List<RayCast2D> RayCast2DWallChecksLeft  { get; }

	// Right wall checks
	public List<RayCast2D> RayCast2DWallChecksRight { get; }

	// Is entity within wall jump-able area
	public bool InWallJumpArea { get; }

	// Wall direction
	public int WallDir { get; set; }

	// Is the entity falling?
	bool IsFalling();
}

public class EntityCommandWallJump : EntityCommand<IEntityWallJumpable>
{
	public EntityCommandWallJump(IEntityWallJumpable entity) : base(entity) { }

	public override void Start()
	{
		// on a wall and falling
		if (Entity.InWallJumpArea && !Entity.IsOnGround())
		{
			if (Entity.WallDir != 0 && Entity.IsFalling())
			{
				var JumpForceWallHorz = 700;
				var JumpForceWallVert = 600;

				// wall jump
				var velocity = Entity.Velocity;
				velocity.x += -JumpForceWallHorz * Entity.WallDir;
				velocity.y -= JumpForceWallVert;
				Entity.Velocity = velocity;
			}
		}
		else
		{
			//It basically just jumps
			var velocity = Entity.Velocity;
			velocity.y -= 600;
			Entity.Velocity = velocity;
		}
	}

	public override void Update(float delta)
	{
		Entity.WallDir = UpdateWallDirection();

		if (Entity.WallDir != 0 && Entity.InWallJumpArea)
		{
			if (Entity.IsFalling())
			{
				var velocity = Entity.Velocity;
				velocity.y = 1;

				// fast fall
				if (Entity is Player player)
					if (player.PlayerInput.IsDown)
						velocity.y += 200;

				Entity.Velocity = velocity;
			}
		}
	}

	private int UpdateWallDirection()
	{
		var left = CollectionExtensions.IsAnyRayCastColliding(Entity.RayCast2DWallChecksLeft);
		var right = CollectionExtensions.IsAnyRayCastColliding(Entity.RayCast2DWallChecksRight);

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}
}
