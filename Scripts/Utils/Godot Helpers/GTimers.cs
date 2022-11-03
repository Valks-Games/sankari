namespace Sankari;

public class GTimers
{
	private Node Node { get; set; }

	public GTimers(Node node) => Node = node;

	public GTimer CreateTimer(int delayMS, bool loop, bool autoStart) =>
		new GTimer(Node, delayMS, loop, autoStart);

	public GTimer CreateTimer(Callable callable, int delayMS, bool loop, bool autoStart) =>
		new GTimer(Node, callable, delayMS, loop, autoStart);
}

