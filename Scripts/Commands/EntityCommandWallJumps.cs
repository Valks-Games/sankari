namespace Sankari;

public interface IEntityWallJumpable : IEntityMoveable
{
	// Horizontal wall jump force
	public int JumpForceWallHorz { get; }

	// Vertical wall jump force
	public int JumpForceWallVert { get; }

	// Is entity within wall jump-able area
	public bool InWallJumpArea { get; }

	// Wall direction
	public int WallDir { get; }

	// Sprite to modify
	public AnimatedSprite2D AnimatedSprite { get; }

	// Is the entity falling?
	bool IsFalling();

	// Force the entity to jump
	void Jump();	
}
public class EntityCommandWallJumps : EntityCommand<IEntityWallJumpable>
{
	public EntityCommandWallJumps(IEntityWallJumpable entity) : base(entity) { }

	public override void Update(MovementInput input)
	{
		var velocity = Entity.Velocity;
		// on a wall and falling
		if (Entity.WallDir != 0 && Entity.InWallJumpArea)
		{
			Entity.AnimatedSprite.FlipH = Entity.WallDir == 1;

			if (Entity.IsFalling())
			{
				velocity.y = 0;

				// fast fall
				if (input.IsDown)
					velocity.y += 50;

				// wall jump
				if (input.IsJump)
				{
					Entity.Jump();
					velocity.x += -Entity.JumpForceWallHorz * Entity.WallDir;
					velocity.y -= Entity.JumpForceWallVert;
				}
			}
		}
		else
			Entity.AnimatedSprite.FlipH = false;

		Entity.Velocity = velocity;
	}
}
