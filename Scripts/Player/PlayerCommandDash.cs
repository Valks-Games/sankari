namespace Sankari;

public class PlayerCommandDash : PlayerCommand
{
	public Vector2 DashDir          { get; set; }
	public int     MaxDashes        { get; set; } = 1;
	public int     DashCount        { get; set; }
	public bool    HorizontalDash   { get; set; }
	public bool    DashReady        { get; set; } = true;
	public bool    CurrentlyDashing { get; set; }
	public int DashCooldown           { get; set; }	= 1400;
	public int DashDuration           { get; set; }	= 800;

	public GTimer TimerDashCooldown { get; set; }
	public GTimer TimerDashDuration { get; set; }

	public override void Init(Player player)
	{
		TimerDashCooldown = new GTimer(player, nameof(OnDashReady), DashCooldown, false, false);
		TimerDashDuration = new GTimer(player, nameof(OnDashDurationDone), DashDuration, false, false);
	}

	public override void Update(Player player) 
	{
		if (player.IsOnGround())
			DashCount = 0;

		if (player.InputDash && DashReady && !CurrentlyDashing && DashCount != MaxDashes && !player.IsOnGround())
		{
			DashDir = GetDashDirection(player);

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
			sprite.Texture = player.AnimatedSprite.Frames.GetFrame(player.AnimatedSprite.Animation, player.AnimatedSprite.Frame);
			sprite.GlobalPosition = player.GlobalPosition;
			sprite.Scale = new Vector2(2f, 2f);
			sprite.FlipH = player.AnimatedSprite.FlipH;
			//sprite.FlipH = wallDir == 1 ? true : false;
			player.Tree.AddChild(sprite);

			var dashSpeed = player.SpeedDashVertical;

			if (HorizontalDash)
				dashSpeed = player.SpeedDashHorizontal;

			player.velocityPlayer = DashDir * dashSpeed;
		}	
	}

	private Vector2 GetDashDirection(Player player)
	{
		if (player.InputDown && player.MoveDir.x < 0)
		{
			return new Vector2(-1, 1);
		}
		else if (player.InputDown && player.MoveDir.x == 0)
		{
			HorizontalDash = false;
			return new Vector2(0, 1);
		}
		else if (player.InputDown && player.MoveDir.x > 0)
		{
			return new Vector2(1, 1);
		}
		else if (player.InputUp && player.MoveDir.x < 0)
		{
			HorizontalDash = false;
			return new Vector2(-1, -1);
		}
		else if (player.InputUp && player.MoveDir.x > 0)
		{
			HorizontalDash = false;
			return new Vector2(1, -1);
		}
		else if (player.InputUp)
		{
			HorizontalDash = false;
			return new Vector2(0, -1);
		}
		else if (player.MoveDir.x < 0)
		{
			HorizontalDash = true;
			return new Vector2(-1, 0);
		}
		else if (player.MoveDir.x > 0)
		{
			HorizontalDash = true;
			return new Vector2(1, 0);
		}

		return Vector2.Zero;
	}

	private void OnDashReady() => DashReady = true;
	private void OnDashDurationDone() => CurrentlyDashing = false;
}
