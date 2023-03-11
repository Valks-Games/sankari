namespace Sankari;

public class EntityAnimationJumpFall<T> : EntityAnimation<T> where T : MovingEntity<T>
{
    public EntityAnimationJumpFall(T entity) : base(entity) { }

    public override void Enter() => Entity.AnimatedSprite.Play("jump_fall");

    /// <summary>
    /// <br>JumpFall -> Idle</br>
    /// <br>JumpFall -> JumpStart</br>
    /// </summary>
    public override void HandleTransitions()
    {
        if (Entity.IsNearGround())
            HandleTransitionsNearGround();
        else if (!Entity.IsFalling() && Entity.Velocity.Y != 0)
            HandleTransitionsFalling();
    }

    public virtual void HandleTransitionsNearGround() =>
        SwitchState(EntityAnimationType.Idle);    

    public virtual void HandleTransitionsFalling() =>
        SwitchState(EntityAnimationType.JumpStart);
}
