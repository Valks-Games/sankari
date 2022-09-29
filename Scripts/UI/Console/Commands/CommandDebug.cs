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

        Commands["peerid"] = () => 
        {
            if (!Net.Client.IsRunning)
            {
                Logger.Log("Client is not running!");
                return;
            }

            // WARN: Not a thread safe way to get peerId
            Logger.Log(Net.Client.PeerId);
        };

        Commands["players"] = () => 
        {
            if (!Net.Server.IsRunning)
            {
                Logger.Log("Server is not running on this client");
                return;
            }

            // WARN: Not a thread safe way to get Players
            Logger.Log(Net.Server.Players.PrintFull());
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
