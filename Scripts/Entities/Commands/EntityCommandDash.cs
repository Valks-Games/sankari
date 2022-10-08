namespace Sankari;

public interface IEntityDashable : IEntityMoveable
{
	// Max speed on the ground while sprinting
	public int SpeedMaxGroundSprint { get; }

	// Max speed on the ground while walking
	public int SpeedMaxGround { get; }

	// Max speed when in the air
	public int SpeedMaxAir { get; }

	// Speed given when dashing vertically
	public int SpeedDashVertical { get; }

	// Speed given when dashing horizontally
	public int SpeedDashHorizontal { get; }

	// Is the entity on the ground?

	// Sprite to modify
	public AnimatedSprite2D AnimatedSprite { get; }

	bool IsOnGround();
}

public class EntityCommandDash : EntityCommand<IEntityDashable>
{
	public EntityCommandDash(IEntityDashable entity) : base(entity)
	{
	}

	private Vector2 DashDir          { get; set; }
	private int     MaxDashes        { get; set; } = 1;
	private int     DashCount        { get; set; }
	private bool    HorizontalDash   { get; set; }
	private bool    DashReady        { get; set; } = true;
	private bool    CurrentlyDashing { get; set; }
	private int DashCooldown           { get; set; }	= 1400;
	private int DashDuration           { get; set; }	= 800;

	private GTimer TimerDashCooldown { get; set; }
	private GTimer TimerDashDuration { get; set; }

	public override void Initialize()
	{
		TimerDashCooldown = Entity.Timers.CreateTimer(new Callable(OnDashReady), DashCooldown, false, false);
		TimerDashDuration = Entity.Timers.CreateTimer(new Callable(OnDashDurationDone), DashDuration, false, false);
	}

	public override void Start()
	{
		if (DashReady && !CurrentlyDashing && DashCount != MaxDashes && !Entity.IsOnGround())
		{
			DashDir = GetDashDirection(Entity.MoveDir);

			if (DashDir != Vector2.Zero)
			{
				DashCount++;
				Audio.PlaySFX("dash");
				DashReady = false;
				CurrentlyDashing = true;
				TimerDashDuration.Start();
				TimerDashCooldown.Start();
			}
		}
	}

	public override void Update(MovementInput input)
	{
		// Entity.IsOnGround() is called twice, in Update() and LateUpdate() 
		// Also what if IsOnGround() was called in other commands? 
		// Shouldn't IsOnGround() only be called once?
		if (Entity.IsOnGround())
			DashCount = 0;

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

	public override void LateUpdate(MovementInput input)
	{
		var velocity = Entity.Velocity;

		// None of this is dash related code, it should be somewhere else
		// Perhaps this should be moved to something like EntityCommandMove
		if (Entity.IsOnGround() && input.IsSprint)
		{
			Entity.AnimatedSprite.SpeedScale = 1.5f;
			velocity.x = Mathf.Clamp(Entity.Velocity.x, -Entity.SpeedMaxGroundSprint, Entity.SpeedMaxGroundSprint);
		}
		else
		{
			Entity.AnimatedSprite.SpeedScale = 1;
			velocity.x = Mathf.Clamp(Entity.Velocity.x, -Entity.SpeedMaxGround, Entity.SpeedMaxGround);
		}

		velocity.y = Mathf.Clamp(Entity.Velocity.y, -Entity.SpeedMaxAir, Entity.SpeedMaxAir);

		Entity.Velocity = velocity;
	}

	public override void Stop()
	{
		if (CurrentlyDashing)
		{
			TimerDashDuration.Stop();
			CurrentlyDashing = false;
		}
	}

	private Vector2 GetDashDirection(Vector2 moveDir)
	{
		// Get vertical dash direction
		float y = 0;
		if (MovementUtils.IsDown(moveDir))
			y = Vector2.Down.y;
		else if (MovementUtils.IsUp(moveDir))
			y = Vector2.Up.y;

		// Get horizontal dash direction
		float x = 0;
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

	private void OnDashDurationDone() => CurrentlyDashing = false;
}
