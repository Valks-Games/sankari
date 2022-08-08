namespace MarioLikeGame;

public class PlayerFallingState : PlayerBaseState 
{
    private const int _fallAirSpeed = 70;

    public override void EnterState(PlayerStateManager manager)
    {
        GD.Print("Falling");
    }

    public override void UpdateState(PlayerStateManager manager)
    {
        manager.HandleMovement(_fallAirSpeed);

        if (manager.IsOnFloor())
        {
            manager.SwitchState(manager.PlayerIdleState);
        }
    }

    public override void OnAreaEntered(PlayerStateManager manager, Area2D area)
    {
        
    }
}
