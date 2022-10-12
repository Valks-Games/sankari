namespace Sankari;

public interface IEntityMovement : IEntityMoveable, IEntityDash
{
	// Ground acceleration
	public int GroundAcceleration { get; set; }

}

public class EntityCommandMovement : EntityCommand<IEntityMovement>
{
	public int MaxSpeedWalk     { get; set; } = 350;
	public int MaxSpeedSprint   { get; set; } = 500;
	public int MaxSpeedAir      { get; set; } = 350;
	public int AirAcceleration  { get; set; } = 20;
	public int DampeningAir     { get; set; } = 10;
	public int DampeningGround  { get; set; } = 25;

	public EntityCommandMovement(IEntityMovement entity) : base(entity) { }

	public override void Initialize()
	{
		// if these are equal to each other then the player movement will not work as expected
		if (Entity.GroundAcceleration == DampeningGround)
			DampeningGround -= 1;
	}

	public override void UpdateSpriteDirection()
	{
		//Entity.AnimatedSprite.FlipH is true when facing to the left, false when facing to the right
		//Entity.MoveDir.x 0 idle -1 to the left 1 to the right
		if (Entity.MoveDir.x != 0)
			Entity.AnimatedSprite.FlipH = (Entity.MoveDir.x > 0) ? false : true;
	}

	public override void UpdateGroundWalking(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x = ClampAndDampen(velocity.x, DampeningGround, MaxSpeedWalk);
		Entity.Velocity = velocity;
		UpdateSpriteDirection();
	}

	public override void UpdateGroundSprinting(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x = ClampAndDampen(velocity.x, DampeningGround, MaxSpeedSprint);
		Entity.Velocity = velocity;
		UpdateSpriteDirection();
	}

	public override void UpdateAir(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x += Entity.MoveDir.x * AirAcceleration;
		velocity.x = ClampAndDampen(velocity.x, DampeningAir, MaxSpeedAir);
		Entity.Velocity = velocity;
		UpdateSpriteDirection();
	}

	private float ClampAndDampen(float horzVelocity, int dampening, int maxSpeedGround) 
	{
		if (horzVelocity > 0)
			return Mathf.Min(horzVelocity - dampening, maxSpeedGround);
		else
			return Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
	}
}
