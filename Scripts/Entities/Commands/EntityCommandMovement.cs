namespace Sankari;

public interface IEntityMovement : IEntityMoveable, IEntityDash
{
	// Ground acceleration
	public int GroundAcceleration { get; set; }
	public int MaxSpeedWalk     { get; set; }
	public int MaxSpeedSprint   { get; set; }
	public int MaxSpeedAir      { get; set; }
	public int AirAcceleration  { get; set; }
	public int DampeningAir     { get; set; }
	public int DampeningGround  { get; set; }
}

public class EntityCommandMovement : EntityCommand<IEntityMovement>
{
	public EntityCommandMovement(IEntityMovement entity) : base(entity) { }

	public override void Initialize()
	{
		// if these are equal to each other then the player movement will not work as expected
		if (Entity.GroundAcceleration == Entity.DampeningGround)
			Entity.DampeningGround -= 1;
	}

	public override void UpdateGroundWalking(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x = ClampAndDampen(velocity.x, Entity.DampeningGround, Entity.MaxSpeedWalk);
		Entity.Velocity = velocity;
	}

	public override void UpdateGroundSprinting(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x = ClampAndDampen(velocity.x, Entity.DampeningGround, Entity.MaxSpeedSprint);
		Entity.Velocity = velocity;
	}

	public override void UpdateAir(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x += Entity.MoveDir.x * Entity.AirAcceleration;
		velocity.x = ClampAndDampen(velocity.x, Entity.DampeningAir, Entity.MaxSpeedAir);
		Entity.Velocity = velocity;
	}

	private float ClampAndDampen(float horzVelocity, int dampening, int maxSpeedGround) 
	{
		if (Mathf.Abs(horzVelocity) <= dampening)
			return 0;
		else if (horzVelocity > 0)
			return Mathf.Min(horzVelocity - dampening, maxSpeedGround);
		else
			return Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
	}
}
