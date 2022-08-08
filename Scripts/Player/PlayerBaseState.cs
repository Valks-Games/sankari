namespace MarioLikeGame;

public abstract class PlayerBaseState
{
    public virtual void EnterState(PlayerStateManager manager) 
    {
        manager.Label.Text = GetType().Name.Replace("Player", "").Replace("State", "");
    }

    public virtual void UpdateState(PlayerStateManager manager) 
    {

    }

    public virtual void OnAreaEntered(PlayerStateManager manager, Area2D area) 
    {

    }
}