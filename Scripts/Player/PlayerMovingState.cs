namespace MarioLikeGame;

public class PlayerMovingState : PlayerBaseState
{
    private const int _moveSpeed = 100;

    public override void EnterState(PlayerStateManager manager)
    {
        base.EnterState(manager);
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        manager.Speed = _moveSpeed;

        if (manager.InputJump)
            manager.SwitchState(manager.PlayerJumpingState);
        else if (!manager.IsMoving())
            manager.SwitchState(manager.PlayerIdleState);
    }

    public override void OnAreaEntered(PlayerStateManager manager, Area2D area)
    {
        
    }
}