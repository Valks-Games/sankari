namespace MarioLikeGame;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerStateManager manager);
    public abstract void UpdateState(PlayerStateManager manager);
    public abstract void OnAreaEntered(PlayerStateManager manager, Area2D area);
}