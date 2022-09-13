namespace Sankari;

public class CommandDebug : Command
{
    private readonly Dictionary<string, Action> commands = new();

    public CommandDebug() 
    {
        Aliases = new[] { "x" };

        commands["help"] = () => 
        {
            Logger.Log("List of commands:");
            
            foreach (var cmd in commands.Keys)
                Logger.Log(cmd);
        };

        commands["peerid"] = () => 
        {
            if (!GameManager.Net.Client.IsRunning)
            {
                Logger.Log("Client is not running!");
                return;
            }

            Logger.Log(GameManager.Net.Client.PeerId);
        };

        commands["players"] = () => 
        {
            if (!GameManager.Net.Server.IsRunning)
            {
                Logger.Log("Server is not running on this client");
                return;
            }

            Logger.Log(GameManager.Net.Server.Players.PrintFull());
        };

        commands["scroll"] = () => 
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

        commands[args[0]]();
    }
}
