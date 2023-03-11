namespace Sankari;

public class EntityAnimationIdle<T> : EntityAnimation<T> where T : MovingEntity<T>
{
    public EntityAnimationIdle(T entity) : base(entity) { }

    public override void Enter() => Entity.AnimatedSprite.Play("idle");

    /// <summary>
    /// <br>Idle -> JumpStart</br>
    /// <br>Idle -> JumpFall</br>
    /// </summary>
    public override void HandleTransitions()
    {
        if (!Entity.IsNearGround() && Entity.IsFalling())
            HandleTransitionsFalling();
        else if (!Entity.IsNearGround())
            HandleTransitionsNotNearGround();
    }

    public virtual void HandleTransitionsFalling() =>
        SwitchState(EntityAnimationType.JumpFall);

    public virtual void HandleTransitionsNotNearGround() =>
        SwitchState(EntityAnimationType.JumpStart);
}
