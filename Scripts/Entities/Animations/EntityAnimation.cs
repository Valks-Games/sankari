namespace Sankari;

public enum EntityAnimationType
{
    None = 0,
    Idle,
    Walking,
    Running,
    PreJumpStart,
    JumpStart,
    JumpFall,
    Dash
}

public abstract class EntityAnimation<T> where T : MovingEntity<T>
{
    protected T Entity { get; set; }

    public EntityAnimation(T entity)
    {
        Entity = entity;
    }

    //private int Counter { get; set; }

    protected void SwitchState(EntityAnimationType animation)
    {
        //Logger.Log($"[{Counter++}][{Entity}] Switching to {animation}");

        if (!Entity.Animations.ContainsKey(animation))
        {
            Logger.LogWarning($"{animation} for {Entity} has not been setup yet");
            return;
        }

        Entity.Animations[Entity.CurrentAnimation].Exit();
        Entity.CurrentAnimation = animation;
        Entity.Animations[Entity.CurrentAnimation].Enter();
    }

    protected void FlipSpriteOnDirection()
    {
        if (Entity.MoveDir.X != 0)
        {
            Entity.AnimatedSprite.FlipH = Entity.MoveDir.X < 0; // flip sprite if moving left
        }
    }

    /// <summary>
    /// Setup and start the animation
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Update the current animation
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Potentially transition to a new state
    /// </summary>
    public virtual void HandleTransitions() { }

    /// <summary>
    /// Cleanly exit the animation state
    /// </summary>
    public virtual void Exit() { }

    public override string ToString() => GetType().Name.Replace("EntityAnimation", "");
}
