namespace Sankari;

public partial class Slime : CharacterBody2D, IEnemy
{
    private float Gravity { get; set; } = 250f;
    private bool Jumping { get; set; }
    private GTimer JumpTimer { get; set; }
    private bool MovingForward { get; set; }
    private int WallHugTime { get; set; }

	// fields
    private Vector2 velocity;

    public override void _Ready()
    {
        JumpTimer = new GTimer(this, nameof(OnJumpTimer), 2000);
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        velocity.y += delta * Gravity;

        if (IsOnFloor() && !Jumping)
        {
            velocity.x = 0;
            velocity.y = 0;
        }

        if (Jumping)
            Jumping = false;

        if (IsOnWall())
        {
            WallHugTime++;

            if (WallHugTime >= 50)
                MovingForward = !MovingForward;
        }

		Velocity = velocity;

        MoveAndSlide();
    }

    private void OnJumpTimer()
    {
        Jumping = true;
        WallHugTime = 0;
        velocity.x += MovingForward ? 20 : -20;
        velocity.y -= 150;
    }
}
