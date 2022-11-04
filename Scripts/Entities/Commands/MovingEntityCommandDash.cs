namespace Sankari;

public class MovingEntityCommandDash : EntityCommand<MovingEntity>
{
	public int     MaxDashes         { get; set; } = 1; // Max number of allowed dashes before needing to be reset
	public int     DashCooldown      { get; set; }	= 1400; // How long before dashing is available again
	public int     DashDuration      { get; set; }	= 200; // How long the dash lasts for
	public int SpeedDashVertical     { get; set; } = 400; // Vertical dash speed
	public int SpeedDashHorizontal   { get; set; } = 600; // Horizontal dash speed

	public event    EventHandler DashDurationDone;
	public bool     CurrentlyDashing  { get; set; } = false;
	public bool     DashReady         { get; set; } = true;
	public int      DashCount         { get; set; }
	private Vector2 DashDir           { get; set; }
	private bool    HorizontalDash    { get; set; }
	private GTimer  TimerDashCooldown { get; set; }
	private GTimer  TimerDashDuration { get; set; }

	public MovingEntityCommandDash(MovingEntity entity) : base(entity) { }

	public override void Initialize()
	{
		TimerDashCooldown = Entity.Timers.CreateTimer(nameof(Entity.OnDashReady), DashCooldown, false);
		TimerDashCooldown.Loop = false;

		TimerDashDuration = Entity.Timers.CreateTimer(nameof(Entity.OnDashDurationDone), DashDuration, false);
		TimerDashDuration.Loop = false;
	}

	public override void Start()
	{
		if (DashReady && !CurrentlyDashing && DashCount != MaxDashes && !Entity.IsNearGround())
		{
			DashDir = GetDashDirection(Entity.MoveDir);

			if (DashDir != Vector2.Zero)
			{
				GameManager.EventsPlayer.Notify(EventPlayer.OnDash);
				Entity.GravityEnabled = false;
				DashCount++;
				DashReady = false;
				CurrentlyDashing = true;
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
		if (Entity.IsNearGround())
			DashCount = 0;
	}

	public override void UpdateAir(float delta)
	{
		if (CurrentlyDashing)
		{
			var sprite = Prefabs.PlayerDashTrace.Instantiate<Sprite2D>();
			sprite.Texture = Entity.AnimatedSprite.Frames.GetFrame(Entity.AnimatedSprite.Animation, Entity.AnimatedSprite.Frame);
			sprite.GlobalPosition = Entity.GlobalPosition;
			sprite.Scale = new Vector2(2f, 2f); // this is ugly
			sprite.FlipH = Entity.AnimatedSprite.FlipH;
			//sprite.FlipH = wallDir == 1 ? true : false; // cant remember why I commented this out
			Entity.Tree.AddChild(sprite);

			var dashSpeed = SpeedDashVertical;

			if (HorizontalDash)
				dashSpeed = SpeedDashHorizontal;

			Entity.Velocity = DashDir * dashSpeed;
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
}
