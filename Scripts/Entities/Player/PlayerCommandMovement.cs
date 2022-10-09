namespace Sankari;

public class PlayerCommandMovement : PlayerCommand
{
	private int MaxSpeedWalk     { get; set; } = 350;
	private int MaxSpeedSprint   { get; set; } = 500;
	private int MaxSpeedAir      { get; set; } = 350;
	private int AirAcceleration  { get; set; } = 20;
	private int DampeningAir     { get; set; } = 10;
	private int DampeningGround  { get; set; } = 25;

	public PlayerCommandMovement(Player player) : base(player) { }

	public override void Init()
	{
		// if these are equal to each other then the player movement will not work as expected
		if (Player.GroundAcceleration == DampeningGround)
			DampeningGround -= 1;
	}

	public override void Update(float delta)
	{
		UpdateMoveDirection(Player.PlayerInput);
		UpdateUnderPlatform(Player.PlayerInput);
	}

	public override void UpdateGroundWalking()
	{
		Player.PlayerVelocity.x = ClampAndDampen(Player.PlayerVelocity.x, DampeningGround, MaxSpeedWalk);
	}

	public override void UpdateGroundSprinting()
	{
		Player.PlayerVelocity.x = ClampAndDampen(Player.PlayerVelocity.x, DampeningGround, MaxSpeedSprint);
	}

	public override void UpdateAir()
	{
		if (Player.PlayerInput.IsFastFall)
			Player.PlayerVelocity.y += 10;

		Player.PlayerVelocity.x += Player.MoveDir.x * AirAcceleration;
		Player.PlayerVelocity.x = ClampAndDampen(Player.PlayerVelocity.x, DampeningAir, MaxSpeedAir);
	}

	public override void Jump()
	{
		Player.JumpCount++;
		//Player.PlayerVelocity.y = 0; // reset velocity before jump (is this really needed?)
		Player.PlayerVelocity.y -= Player.JumpForce;
	}

	private float ClampAndDampen(float horzVelocity, int dampening, int maxSpeedGround) 
	{
		if (horzVelocity > 0)
			return Mathf.Min(horzVelocity - dampening, maxSpeedGround);
		else
			return Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
	}

	private void UpdateMoveDirection(MovementInput input)
	{
		var x = -Convert.ToInt32(input.IsLeft) + Convert.ToInt32(input.IsRight);
		var y = -Convert.ToInt32(input.IsUp) + Convert.ToInt32(input.IsDown);

		Player.MoveDir = new Vector2(x, y);
	}

	private async void UpdateUnderPlatform(MovementInput input)
	{
		var collision = Player.RayCast2DGroundChecks[0].GetCollider();

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
