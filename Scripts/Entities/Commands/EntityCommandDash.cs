namespace Sankari;

public interface IEntityDash : IEntityMoveable
{
	// Current dash direction
	public Vector2 DashDir           { get; set; }

	// Max number of dashes before entity has to touch the ground
	public int     MaxDashes         { get; set; }

	// The current dash count
	public int     DashCount         { get; set; }

	// Is this entity currently dashing horizontally?
	public bool    HorizontalDash    { get; set; }

	// If true, the entity may dash
	public bool    DashReady         { get; set; }

	// The cooldown between dashes
	public int     DashCooldown      { get; set; }

	// The duration of the dash
	public int     DashDuration      { get; set; }

	// Timer for dash cooldown
	public GTimer  TimerDashCooldown { get; set; }

	// Timer for dash duration
	public GTimer  TimerDashDuration { get; set; }

	// Entity is currently dashing
	public bool    CurrentlyDashing  { get; set; }

	// Timer to prevent going under a platform too early right after the end of a dash
	public GTimer  DontCheckPlatformAfterDashDuration { get; set; }
}

public class EntityCommandDash : EntityCommand<IEntityDash>
{
	public EntityCommandDash(IEntityDash entity) : base(entity) { }

	public override void Initialize()
	{
		Entity.TimerDashCooldown = Entity.Timers.CreateTimer(new Callable(OnDashReady), Entity.DashCooldown, false, false);
		Entity.TimerDashDuration = Entity.Timers.CreateTimer(new Callable(OnDashDurationDone), Entity.DashDuration, false, false);
	}

	public override void Start()
	{
		if (Entity.DashReady && !Entity.CurrentlyDashing && Entity.DashCount != Entity.MaxDashes && !Entity.IsOnGround())
		{
			Entity.DashDir = GetDashDirection(Entity.MoveDir);

			if (Entity.DashDir != Vector2.Zero)
			{
				GameManager.EventsPlayer.Notify(EventPlayer.OnDash);
				Entity.GravityEnabled = false;
				Entity.DashCount++;
				Entity.DashReady = false;
				Entity.CurrentlyDashing = true;
				Entity.TimerDashDuration.Start();
				Entity.TimerDashCooldown.Start();
			}
		}
	}

	public override void Update(float delta)
	{
		// Entity.IsOnGround() is called twice, in Update() and LateUpdate() 
		// Also what if IsOnGround() was called in other commands? 
		// Shouldn't IsOnGround() only be called once?
		if (Entity.IsOnGround())
			Entity.DashCount = 0;
	}

	public override void UpdateAir(float delta)
	{
		if (Entity.CurrentlyDashing)
		{
			var sprite = Prefabs.PlayerDashTrace.Instantiate<Sprite2D>();
			sprite.Texture = Entity.AnimatedSprite.Frames.GetFrame(Entity.AnimatedSprite.Animation, Entity.AnimatedSprite.Frame);
			sprite.GlobalPosition = Entity.GlobalPosition;
			sprite.Scale = new Vector2(2f, 2f); // this is ugly
			sprite.FlipH = Entity.AnimatedSprite.FlipH;
			//sprite.FlipH = wallDir == 1 ? true : false; // cant remember why I commented this out
			Entity.Tree.AddChild(sprite);

			var SpeedDashVertical = 400;
			var SpeedDashHorizontal = 600;

			var dashSpeed = SpeedDashVertical;

			if (Entity.HorizontalDash)
				dashSpeed = SpeedDashHorizontal;

			Entity.Velocity = Entity.DashDir * dashSpeed;
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
			Entity.HorizontalDash = false;
		else if (moveDir.x != 0)
			Entity.HorizontalDash = true;

		return new Vector2(x, y);
	}

	private void OnDashReady() => Entity.DashReady = true;

	private void OnDashDurationDone() 
	{
		Entity.DontCheckPlatformAfterDashDuration.Start();
		Entity.CurrentlyDashing = false;
		Entity.GravityEnabled = true;
	}
}
