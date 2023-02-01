﻿namespace Sankari;

public class GTimers
{
	private Node Node { get; set; }

	public GTimers(Node node) => Node = node;

	public GTimer CreateTimer(int delayMS, bool autoStart = true) =>
		new GTimer(Node, delayMS, autoStart);

	public GTimer CreateTimer(Action action, int delayMS, bool autoStart = true) =>
		new GTimer(Node, action, delayMS, autoStart);
}

