namespace Sankari;

public class EntityAnimationNone<T> : EntityAnimation<T> where T : MovingEntity<T>
{
    public EntityAnimationNone(T entity) : base(entity) { }
}
