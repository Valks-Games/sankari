namespace MarioLikeGame;

public class PlayerJumpingState : PlayerBaseState
{
    private const int _jumpForce = -100;
    private const int _jumpAirSpeed = 80;

    public override void EnterState(PlayerStateManager manager)
    {
        GD.Print("Jumping");

        if (!manager.IsOnFloor()) 
        {
            manager.SwitchState(manager.PlayerIdleState);
            return;
        }

        manager.Velocity.y += _jumpForce;
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        manager.HandleMovement(_jumpAirSpeed);

        if (manager.IsFalling())
            manager.SwitchState(manager.PlayerFallingState);
    }

    public override void OnAreaEntered(PlayerStateManager manager, Area2D area)
    {
        
    }
}