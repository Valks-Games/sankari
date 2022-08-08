namespace MarioLikeGame;

public class PlayerMovingState : PlayerBaseState
{
    private const int _moveSpeed = 100;

    public override void EnterState(PlayerStateManager manager)
    {
        GD.Print("Moving");
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        var left = manager.InputLeft;
        var right = manager.InputRight;

        if (left) 
            manager.Velocity.x -= _moveSpeed;

        if (right)
            manager.Velocity.x += _moveSpeed;

        if (manager.InputJump)
            manager.SwitchState(manager.PlayerJumpingState);
        else if (!left && !right)
            manager.SwitchState(manager.PlayerIdleState);
    }

    public override void OnAreaEntered(PlayerStateManager manager, Area2D area)
    {
        
    }
}