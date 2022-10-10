namespace Sankari;

public class GTimers
{
	private Node Node { get; set; }

	public GTimers(Node node) => Node = node;

	/// <inheritdoc cref="GTimer.GTimer(Node, string, int, bool, bool)"/>
	public GTimer CreateTimer(string methodName, int delayMs, bool loop, bool autoStart) =>
		new GTimer(Node, methodName, delayMs, loop, autoStart);

	/// <inheritdoc cref="GTimer.GTimer(Node, Callable, int, bool, bool)"/>
	public GTimer CreateTimer(Callable callable, int delayMS, bool loop, bool autoStart) =>
		new GTimer(Node, callable, delayMS, loop, autoStart);
}

