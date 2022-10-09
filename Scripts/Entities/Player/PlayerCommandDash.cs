namespace Sankari;

public class PlayerCommandDash : PlayerCommand
{
	private Vector2 DashDir          { get; set; }
	private int     MaxDashes        { get; set; } = 1;
	private int     DashCount        { get; set; }
	private bool    HorizontalDash   { get; set; }
	private bool    DashReady        { get; set; } = true;
	private int DashCooldown           { get; set; }	= 1400;
	private int DashDuration           { get; set; }	= 200;

	private GTimer TimerDashCooldown { get; set; }
	private GTimer TimerDashDuration { get; set; }

	public PlayerCommandDash(Player player) : base(player) { }

	public override void Initialize()
	{
		TimerDashCooldown = Player.Timers.CreateTimer(new Callable(OnDashReady), DashCooldown, false, false);
		TimerDashDuration = Player.Timers.CreateTimer(new Callable(OnDashDurationDone), DashDuration, false, false);
	}

	public override void Start()
	{
		if (DashReady && !Player.CurrentlyDashing && DashCount != MaxDashes && !Player.IsOnGround())
		{
			DashDir = GetDashDirection(Player.MoveDir);

			if (DashDir != Vector2.Zero)
			{
				Player.GravityEnabled = false;
				DashCount++;
				Audio.PlaySFX("dash");
				DashReady = false;
				Player.CurrentlyDashing = true;
				TimerDashDuration.Start();
				TimerDashCooldown.Start();
			}
		}
	}

	public override void Update(float delta)
	{
		// Entity.IsOnGround() is called twice, in Update() and LateUpdate() 
		// Also what if IsOnGround() was called in other commands? 
		// Shouldn't IsOnGround() only be called once?
		if (Player.IsOnGround())
			DashCount = 0;
	}

	public override void UpdateAir(float delta)
	{
		if (Player.CurrentlyDashing)
		{
			var sprite = Prefabs.PlayerDashTrace.Instantiate<Sprite2D>();
			sprite.Texture = Player.AnimatedSprite.Frames.GetFrame(Player.AnimatedSprite.Animation, Player.AnimatedSprite.Frame);
			sprite.GlobalPosition = Player.GlobalPosition;
			sprite.Scale = new Vector2(2f, 2f); // this is ugly
			sprite.FlipH = Player.AnimatedSprite.FlipH;
			//sprite.FlipH = wallDir == 1 ? true : false; // cant remember why I commented this out
			Player.Tree.AddChild(sprite);

			var SpeedDashVertical = 400;
			var SpeedDashHorizontal = 600;

			var dashSpeed = SpeedDashVertical;

			if (HorizontalDash)
				dashSpeed = SpeedDashHorizontal;

			Player.PlayerVelocity = DashDir * dashSpeed;
		}
	}

	private Vector2 GetDashDirection(Vector2 moveDir)
	{
		// Get vertical dash direction
		var y = 0f;
		if (MovementUtils.IsDown(moveDir))
			y = Vector2.Down.y;
		else if (MovementUtils.IsUp(moveDir))
			y = Vector2.Up.y;

		// Get horizontal dash direction
		var x = 0f;
		if (moveDir.x != 0)
			x = moveDir.x > 0 ? 1 : -1;

		// Only update horizontal dash property if input for it is received
		if (MovementUtils.IsUp(moveDir) || (MovementUtils.IsDown(moveDir) && moveDir.x == 0))
			// Prioritize input up for vertical dashing
			HorizontalDash = false;
		else if (moveDir.x != 0)
			HorizontalDash = true;

		return new Vector2(x, y);
	}

	private void OnDashReady() => DashReady = true;

	private void OnDashDurationDone() 
	{
		Player.DontCheckPlatformAfterDashDuration.Start();
		Player.CurrentlyDashing = false;
		Player.GravityEnabled = true;
	}
}
