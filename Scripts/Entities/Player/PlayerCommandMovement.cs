namespace Sankari;

public interface IEntityMovement 
{
	// Maximum walking speed
	public int MaxSpeedWalk { get; set; }

	// Maximum sprinting speed
	public int MaxSpeedSprint   { get; set; }

	// Maximum airial speed
	public int MaxSpeedAir      { get; set; }

	// How fast the entity speeds up
	public int AirAcceleration  { get; set; }

	// How much the air effects movement
	public int DampeningAir     { get; set; }

	// How much the ground effects movement
	public int DampeningGround  { get; set; }

}

public class PlayerCommandMovement : PlayerCommand, IEntityMovement
{
	public int MaxSpeedWalk     { get; set; } = 350;
	public int MaxSpeedSprint   { get; set; } = 500;
	public int MaxSpeedAir      { get; set; } = 350;
	public int AirAcceleration  { get; set; } = 20;
	public int DampeningAir     { get; set; } = 10;
	public int DampeningGround  { get; set; } = 25;

	public PlayerCommandMovement(Player player) : base(player) { }

	public override void Initialize()
	{
		// if these are equal to each other then the player movement will not work as expected
		if (Entity.GroundAcceleration == DampeningGround)
			DampeningGround -= 1;
	}

	public override void Update(float delta)
	{
		if (!Entity.CurrentlyDashing && !Entity.DontCheckPlatformAfterDashDuration.IsActive())
			UpdateUnderPlatform(Entity.PlayerInput);
	}

	public override void UpdateGroundWalking(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x = ClampAndDampen(velocity.x, DampeningGround, MaxSpeedWalk);
		Entity.Velocity = velocity;
	}

	public override void UpdateGroundSprinting(float delta)
	{
		var velocity = Entity.Velocity;
		velocity.x = ClampAndDampen(velocity.x, DampeningGround, MaxSpeedSprint);
		Entity.Velocity = velocity;
	}

	public override void UpdateAir(float delta)
	{
		var velocity = Entity.Velocity;
		if (Entity.PlayerInput.IsFastFall)
			velocity.y += 10;

		velocity.x += Entity.MoveDir.x * AirAcceleration;
		velocity.x = ClampAndDampen(velocity.x, DampeningAir, MaxSpeedAir);
		Entity.Velocity = velocity;
	}

	private float ClampAndDampen(float horzVelocity, int dampening, int maxSpeedGround) 
	{
		if (horzVelocity > 0)
			return Mathf.Min(horzVelocity - dampening, maxSpeedGround);
		else
			return Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
	}

	private async void UpdateUnderPlatform(MovementInput input)
	{
		var collision = Entity.RayCast2DGroundChecks[0].GetCollider();

		if (collision is TileMap tilemap)
		{
			if (input.IsDown && tilemap.IsInGroup("Platform"))
			{
				tilemap.EnableLayers(2);
				await Task.Delay(1000);
				tilemap.EnableLayers(1, 2);
			}
		}
	}
}
