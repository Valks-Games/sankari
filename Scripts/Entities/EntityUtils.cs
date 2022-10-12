namespace Sankari;

public interface IEntityBase
{
	public Vector2 MoveDir { get; set; }
	public GTimers Timers { get; set; }
}

internal class EntityUtils
{
}
