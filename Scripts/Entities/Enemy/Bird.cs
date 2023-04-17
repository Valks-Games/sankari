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
        TimerChangeDirection = new GTimer(this, 1000);
        TimerFlap = new GTimer(this, OnTimerFlap, 1000) { Loop = true };
        TimerFlap.StartMs();
        AnimatedSprite.Play("fly");
        MoveDir = Vector2.Left;
    }

    public override void UpdatePhysics()
    {
        if (IsOnWall() && !TimerChangeDirection.IsActive())
        {
            TimerChangeDirection.StartMs();
            MoveDir = new Vector2(-MoveDir.X, MoveDir.Y);
            AnimatedSprite.FlipH = MoveDir.X == 1;
        }
    }

    private void OnTimerFlap() 
    {
        Velocity = Velocity + new Vector2(40 * MoveDir.X, -80);
    }

    private void _on_damage_area_entered(Area2D area)
    {
        if (area.GetParent() is Player player)
            player.RemoveHealth(1);
    }
}
