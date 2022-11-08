namespace Sankari;

public partial class Slime : MovingEntity
{
    public override int Gravity { get; set; } = 250;
	public override bool ClampDampenAir { get; set; } = false;
	public override int  DampeningGround { get; set; } = 2;

    private bool Jumping { get; set; }
	private GTimer IdleTimer { get; set; }
	private GTimer PreJumpTimer { get; set; }
    private GTimer JumpTimer { get; set; }
    private bool MovingForward { get; set; }
    private int WallHugTime { get; set; }
	private bool CanJump { get; set; } = true;
	private bool StartedPreJump { get; set; }

    public override void Init()
    {
		Animations[EntityAnimationType.Idle]      = new EntityAnimationIdle<MovingEntity>(this);
		Animations[EntityAnimationType.JumpStart] = new EntityAnimationJumpStart<MovingEntity>(this);
		Animations[EntityAnimationType.JumpFall]  = new EntityAnimationJumpFall<MovingEntity>(this);

		AnimatedSprite.Animation = "idle";
		CurrentAnimation = EntityAnimationType.Idle;

		IdleTimer = new GTimer(this, 1000) { Loop = false };
		PreJumpTimer = new GTimer(this, nameof(OnPreJumpTimer), 400, false) { Loop = false };
        JumpTimer = new GTimer(this, nameof(OnJumpTimer), 2000, false) { Loop = false };

		//Label.Visible = true;
    }

	public override void UpdatePhysics()
    {
		Label.Text = "" + CurrentAnimation;

		if (IdleTimer.IsActive())
			return;

        if (Jumping) // this seems unoptimal to check this every frame
            Jumping = false;

        if (IsOnWall())
        {
            WallHugTime++;

            if (WallHugTime >= 50)
                MovingForward = !MovingForward;
        }

		if (StartedPreJump)
		{
			var shakeStrength = 0.4f;

			AnimatedSprite.Offset = new Vector2(-shakeStrength + new Random().NextSingle() * shakeStrength * 2, 0);
		}

		// this seems like it could be improved for performance, this is being checked every frame
		if (IsOnFloor() && CanJump && !StartedPreJump)
		{
			StartedPreJump = true;
			AnimatedSprite.Play("pre_jump_start");
			PreJumpTimer.Start();
		}
    }

	public override void TouchedGround()
	{
		IdleTimer.Start();
	}

	private void OnPreJumpTimer()
	{
		AnimatedSprite.Offset = Vector2.Zero;
		StartedPreJump = false;
		CanJump = false;
		OnJump();
		Jumping = true;
		WallHugTime = 0;

		Velocity = Velocity + new Vector2(MovingForward ? 40 : -40, -300);

		JumpTimer.Start();
	}

	private void OnJumpTimer()
    {
		CanJump = true;
    }

	private void _on_enemy_area_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
			player.RemoveHealth(1);
	}
}
