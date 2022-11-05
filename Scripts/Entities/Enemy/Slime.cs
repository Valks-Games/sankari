namespace Sankari;

public partial class Slime : MovingEntity
{
    public override int Gravity { get; set; } = 250;
	public override bool ClampDampen { get; set; } = false;

    private bool Jumping { get; set; }
	private GTimer PreJumpTimer { get; set; }
    private GTimer JumpTimer { get; set; }
    private bool MovingForward { get; set; }
    private int WallHugTime { get; set; }
	private bool CanJump { get; set; }
	private bool StartedPreJump { get; set; }

    public override void Init()
    {
		Animations[EntityAnimationType.Idle]      = new EntityAnimationIdle(this);
		Animations[EntityAnimationType.JumpStart] = new EntityAnimationJumpStart(this);
		Animations[EntityAnimationType.JumpFall]  = new EntityAnimationJumpFall(this);

		AnimatedSprite.Animation = "idle";
		CurrentAnimation = EntityAnimationType.Idle;

		PreJumpTimer = new GTimer(this, nameof(OnPreJumpTimer), 400)
		{
			Loop = false
		};

        JumpTimer = new GTimer(this, nameof(OnJumpTimer), 2000)
		{
			Loop = false
		};

		Label.Visible = true;
    }

    public override void UpdatePhysics()
    {
		Label.Text = "" + CurrentAnimation;

        if (IsOnFloor() && !Jumping)
			Velocity = Vector2.Zero;

        if (Jumping) // this seems unoptimal to check this every frame
            Jumping = false;

        if (IsOnWall())
        {
            WallHugTime++;

            if (WallHugTime >= 50)
                MovingForward = !MovingForward;
        }

		// this seems like it could be improved for performance, this is being checked every frame
		if (IsOnFloor() && CanJump && !StartedPreJump)
		{
			StartedPreJump = true;
			AnimatedSprite.Play("pre_jump_start");
			PreJumpTimer.Start();
		}
    }

	private void OnPreJumpTimer()
	{
		StartedPreJump = false;
		CanJump = false;
		OnJump();
		Jumping = true;
		WallHugTime = 0;

		Velocity = Velocity + new Vector2(MovingForward ? 20 : -20, -300);

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
