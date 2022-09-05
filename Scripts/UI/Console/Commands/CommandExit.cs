namespace Sankari;

public class CommandExit : Command
{
    public CommandExit() => Aliases = new[] { "stop" };

    public override void Run(string[] args)
    {
        Logger.LogTodo("This has not been implemented yet");
    }
}
