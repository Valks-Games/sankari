namespace Sankari;

public class CommandDebug : Command
{
    private Dictionary<string, Action> Commands { get; } = new();

    public CommandDebug() 
    {
        Aliases = new[] { "x" };

        Commands["help"] = () => 
        {
            Logger.Log("List of commands:");
            
            foreach (var cmd in Commands.Keys)
                Logger.Log(cmd);
        };

        Commands["scroll"] = () => 
        {
            GameManager.Console.ScrollToBottom = !GameManager.Console.ScrollToBottom;
        };
    }

    public override void Run(string[] args)
    {
        if (args.Length == 0)
        {
            Logger.Log("Usage: debug [cmd] (use 'debug help' for a list of commands");
            return;
        }

        Commands[args[0]]();
    }
}
