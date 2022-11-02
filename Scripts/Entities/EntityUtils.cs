namespace Sankari;

public interface IEntityBase
{
	public bool Debug { get; }
	public Vector2 MoveDir { get; }
	public GTimers Timers { get; }
}
