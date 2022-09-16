namespace Sankari;

public partial class Slime : CharacterBody2D, IEnemy
{
    private const float gravity = 250f;
    private Vector2 velocity;
    private bool jumping;
    private GTimer jumpTimer;
    private bool movingForward;
    private int wallHugTime;

    public override void _Ready()
    {
        jumpTimer = new GTimer(this, nameof(OnJumpTimer), 2000);
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        velocity.y += delta * gravity;

        if (IsOnFloor() && !jumping)
        {
            velocity.x = 0;
            velocity.y = 0;
        }

        if (jumping)
            jumping = false;

        if (IsOnWall())
        {
            wallHugTime++;

            if (wallHugTime >= 50)
                movingForward = !movingForward;
        }

        MoveAndSlide();
    }

    private void OnJumpTimer()
    {
        jumping = true;
        wallHugTime = 0;
        velocity.x += movingForward ? 20 : -20;
        velocity.y -= 150;
    }
}
