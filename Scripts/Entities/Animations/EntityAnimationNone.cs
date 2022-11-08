namespace Sankari;

public class EntityAnimationNone<T> : EntityAnimation<T> where T : MovingEntity
{
	public EntityAnimationNone(T entity) : base(entity) { }
}
