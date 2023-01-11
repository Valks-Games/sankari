namespace Sankari;

public partial class Bird : MovingEntity<Bird>
{
	public override int Gravity { get; set; } = 100;
	public override bool ClampDampenGround { get; set; } = false;
	public override bool ClampDampenAir { get; set; } = false;

	private GTimer TimerChangeDirection { get; set; }
	private GTimer TimerFlap { get; set; }

	public override void Init()
	{
		TimerChangeDirection = new GTimer(this, 1000, false);
		TimerFlap = new GTimer(this, nameof(OnTimerFlap), 1000, true) { Loop = true };
		AnimatedSprite.Play("fly");
		MoveDir = Vector2.Left;
	}

	public override void UpdatePhysics()
	{
		if (IsOnWall() && !TimerChangeDirection.IsActive())
		{
			TimerChangeDirection.Start();
			MoveDir = new Vector2(-MoveDir.x, MoveDir.y);
			AnimatedSprite.FlipH = MoveDir.x == 1;
		}
	}

	private void OnTimerFlap() 
	{
		Velocity = Velocity + new Vector2(40 * MoveDir.x, -80);
	}

	private void _on_damage_area_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
			player.RemoveHealth(1);
	}
}
