using Sankari.Scripts.Player.Movement;

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
		TimerDashCooldown = new GTimer(player, new Callable(OnDashReady), DashCooldown, false, false);
		TimerDashDuration = new GTimer(player, new Callable(OnDashDurationDone), DashDuration, false, false);
	}

	public override void Update(Player player, MovementInput input)
	{
		if (player.IsOnGround())
			DashCount = 0;

		if (input.IsDash && DashReady && !CurrentlyDashing && DashCount != MaxDashes && !player.IsOnGround())
		{
			DashDir = GetDashDirection(player, input, player.MoveDir);

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

	public override void LateUpdate(Player player, MovementInput input)
	{
		if (player.IsOnGround() && input.IsSprint)
		{
			player.AnimatedSprite.SpeedScale = 1.5f;
			player.velocityPlayer.x = Mathf.Clamp(player.velocityPlayer.x, -player.SpeedMaxGroundSprint, player.SpeedMaxGroundSprint);
		}
		else
		{
			player.AnimatedSprite.SpeedScale = 1;
			player.velocityPlayer.x = Mathf.Clamp(player.velocityPlayer.x, -player.SpeedMaxGround, player.SpeedMaxGround);
		}

		player.velocityPlayer.y = Mathf.Clamp(player.velocityPlayer.y, -player.SpeedMaxAir, player.SpeedMaxAir);
	}

	private Vector2 GetDashDirection(Player player, MovementInput input, Vector2 moveDir)
	{
		// We move vertically direction we are pressing, default to down
		float y = 0;
		if (input.IsDown)
		{
			y = 1;
		}
		else if (input.IsUp)
		{
			y = -1;
		}

		// We move horizontally in the direction we are moving (Wow)
		float x = 0;
		if (moveDir.x != 0)
		{
			x = moveDir.x > 0 ? 1 : -1;
		}

		// Check if we are doing a horizontal dash. If we can't tell, we don't need to update Hortizontal Dash
		if (input.IsUp || (input.IsDown && moveDir.x == 0))
		{
			// We prioritize input up for vertical dashing
			HorizontalDash = false;
		}
		else if (moveDir.x != 0)
		{
			HorizontalDash = true;
		}

		return new Vector2(x, y);
	}

	private void OnDashReady() => DashReady = true;

	private void OnDashDurationDone() => CurrentlyDashing = false;
}
