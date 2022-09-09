using Sankari.Netcode.Client;
using Sankari.Netcode.Server;

namespace Sankari;

public class CommandDebug : Command
{
    public CommandDebug() => Aliases = new[] { "x" };

    public override void Run(string[] args)
    {
        if (args.Length == 0) 
        {
            Logger.Log("Please specify an argument");
            return;
        }

        if (args[0] == "peerid") 
        {
            Logger.Log(GameManager.Net.Client.PeerId);
        }

        if (args[0] == "players")
        {
            if (!GameManager.Net.Server.IsRunning) 
            {
                Logger.Log("Server is not running on this client");
                return;
            }

            Logger.Log(GameManager.Net.Server.Players.PrintFull());
        }
    }
}
