namespace Sankari;

public interface IEntityDash : IEntityMoveable
{
	public int MaxDashes           { get; set; }
	public int DashCooldown        { get; set; }
	public int DashDuration        { get; set; }
	public int SpeedDashVertical   { get; set; }
	public int SpeedDashHorizontal { get; set; }
}

public class EntityCommandDash : EntityCommand<IEntityDash>
{
	public Vector2 DashDir           { get; set; }
	public int     DashCount         { get; set; }
	public bool    HorizontalDash    { get; set; }
	public bool    DashReady         { get; set; } = true;
	public GTimer  TimerDashCooldown { get; set; }
	public GTimer  TimerDashDuration { get; set; }
	public bool    CurrentlyDashing  { get; set; } = false;

	public event EventHandler DashDurationDone;

	public EntityCommandDash(IEntityDash entity) : base(entity) { }

	public override void Initialize()
	{
		TimerDashCooldown = Entity.Timers.CreateTimer(new Callable(OnDashReady), Entity.DashCooldown, false, false);
		TimerDashDuration = Entity.Timers.CreateTimer(new Callable(OnDashDurationDone), Entity.DashDuration, false, false);
	}

	public override void Start()
	{
		if (DashReady && !CurrentlyDashing && DashCount != Entity.MaxDashes && !Entity.IsOnGround())
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
		if (Entity.IsOnGround())
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

			var dashSpeed = Entity.SpeedDashVertical;

			if (HorizontalDash)
				dashSpeed = Entity.SpeedDashHorizontal;

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

	private void OnDashReady()
	{
		Audio.PlaySFX("dash_replenish");
		DashCount = 0; // temporary fix
		DashReady = true;
	}

	private void OnDashDurationDone()
	{
		CurrentlyDashing = false;
		Entity.GravityEnabled = true;
		DashDurationDone?.Invoke(this, EventArgs.Empty);
	}
}
