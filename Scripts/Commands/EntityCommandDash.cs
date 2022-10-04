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

	public override void Update(MovementInput input)
	{
		if (Entity.IsOnGround())
			DashCount = 0;

		if (input.IsDash && DashReady && !CurrentlyDashing && DashCount != MaxDashes && !Entity.IsOnGround())
		{
			DashDir = GetDashDirection(input, Entity.MoveDir);

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

		if (CurrentlyDashing)
		{
			var sprite = Prefabs.PlayerDashTrace.Instantiate<Sprite2D>();
			sprite.Texture = Entity.AnimatedSprite.Frames.GetFrame(Entity.AnimatedSprite.Animation, Entity.AnimatedSprite.Frame);
			sprite.GlobalPosition = Entity.GlobalPosition;
			sprite.Scale = new Vector2(2f, 2f);
			sprite.FlipH = Entity.AnimatedSprite.FlipH;
			//sprite.FlipH = wallDir == 1 ? true : false;
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

	private Vector2 GetDashDirection(MovementInput input, Vector2 moveDir)
	{
		// We move vertically direction we are pressing, default to down
		float y = 0;
		if (input.IsDown)
			y = 1;
		else if (input.IsUp)
			y = -1;

		// We move horizontally in the direction we are moving (Wow)
		float x = 0;
		if (moveDir.x != 0)
			x = moveDir.x > 0 ? 1 : -1;

		// Check if we are doing a horizontal dash. If we can't tell, we don't need to update Hortizontal Dash
		if (input.IsUp || (input.IsDown && moveDir.x == 0))
			// We prioritize input up for vertical dashing
			HorizontalDash = false;
		else if (moveDir.x != 0)
			HorizontalDash = true;

		return new Vector2(x, y);
	}

	private void OnDashReady() => DashReady = true;

	private void OnDashDurationDone() => CurrentlyDashing = false;
}
