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
		Entity.PlayerVelocity.x = ClampAndDampen(Entity.PlayerVelocity.x, DampeningGround, MaxSpeedWalk);
	}

	public override void UpdateGroundSprinting(float delta)
	{
		Entity.PlayerVelocity.x = ClampAndDampen(Entity.PlayerVelocity.x, DampeningGround, MaxSpeedSprint);
	}

	public override void UpdateAir(float delta)
	{
		if (Entity.PlayerInput.IsFastFall)
			Entity.PlayerVelocity.y += 10;

		Entity.PlayerVelocity.x += Entity.MoveDir.x * AirAcceleration;
		Entity.PlayerVelocity.x = ClampAndDampen(Entity.PlayerVelocity.x, DampeningAir, MaxSpeedAir);
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
