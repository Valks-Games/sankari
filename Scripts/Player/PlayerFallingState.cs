namespace MarioLikeGame;

public class PlayerFallingState : PlayerBaseState 
{
    private const int _fallAirSpeed = 70;

    public override void EnterState(PlayerStateManager manager)
    {
        base.EnterState(manager);
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        manager.Speed = _fallAirSpeed;

        if (manager.IsOnFloor())
        {
            manager.SwitchState(manager.PlayerIdleState);
        }
    }

    public override void OnAreaEntered(PlayerStateManager manager, Area2D area)
    {
        
    }
}
