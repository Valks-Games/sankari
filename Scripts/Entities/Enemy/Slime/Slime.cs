namespace Sankari;

public partial class Slime : MovingEntity<Slime>
{
    public override int Gravity { get; set; } = 250;
	public override bool ClampDampenAir { get; set; } = false;
	public override int  DampeningGround { get; set; } = 2;

	public GTimer IdleTimer { get; set; }
	public GTimer PreJumpTimer { get; set; }
    public bool MovingForward { get; set; }
    public int WallHugTime { get; set; }
	public bool StartedPreJump { get; set; }

    public override void Init()
    {
		Animations[EntityAnimationType.Idle]         = new SlimeAnimationIdle(this);
		Animations[EntityAnimationType.PreJumpStart] = new SlimeAnimationPreJumpStart(this);
		Animations[EntityAnimationType.JumpStart]    = new SlimeAnimationJumpStart(this);
		Animations[EntityAnimationType.JumpFall]     = new EntityAnimationJumpFall<Slime>(this);

		AnimatedSprite.Animation = "idle";
		CurrentAnimation = EntityAnimationType.Idle;

		IdleTimer = new GTimer(this, 1000);
		PreJumpTimer = new GTimer(this, nameof(OnPreJumpTimer), 400, false);

		Label.Visible = true;
    }

	public override void UpdatePhysics()
    {
		Label.Text = "" + CurrentAnimation; // this shouldnt be set every frame
    }

	public override void TouchedGround() => IdleTimer.Start();

	private void OnPreJumpTimer()
	{
		OnJump();
		AnimatedSprite.Offset = Vector2.Zero;
		StartedPreJump = false;
		WallHugTime = 0;

		Velocity = Velocity + new Vector2(MovingForward ? 40 : -40, -300);
	}

	private void _on_enemy_area_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
			player.RemoveHealth(1);
	}
}
