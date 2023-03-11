namespace Sankari;

public class PlayerAnimationJumpStart : EntityAnimationJumpStart<Player>
{
    public PlayerAnimationJumpStart(Player player) : base(player) => Entity = player;

    public override void Update()
    {
        FlipSpriteOnDirection();
    }

    public override void HandleTransitions()
    {
        // JumpStart -> Idle
        // JumpStart -> JumpFall
        // JumpStart -> Dash

        base.HandleTransitions();

        if
        (
            Entity.PlayerInput.IsDash &&
            Entity.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady
        )
            SwitchState(EntityAnimationType.Dash);
    }
}
