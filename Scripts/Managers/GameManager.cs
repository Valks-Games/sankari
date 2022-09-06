global using Godot;
global using System;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using System.Linq;

using Sankari.Netcode;

namespace Sankari;

public class GameManager
{
    // managers
    public static TransitionManager Transition { get; private set; }
    public static LevelManager Level { get; private set; }
    public static LevelUIManager LevelUI { get; private set; }
    public static ConsoleManager Console { get; private set; }
    public static Audio Audio { get; private set; }
    public static Popups Popups { get; private set; }
    public static Tokens Tokens { get; private set; }
    public static Net Net { get; private set; }

    private static Node map;
    private static UIMenu menu;

    public GameManager(Linker linker) 
    {
        map = linker.GetNode<Node>("Map");
        menu = linker.GetNode<UIMenu>("CanvasLayer/Menu");
        
        Audio = new Audio(new GAudioStreamPlayer(linker), new GAudioStreamPlayer(linker));
        Transition = linker.GetNode<TransitionManager>(linker.NodePathTransition);
        Console = linker.ConsoleManager;
        LevelUI = linker.GetNode<LevelUIManager>("CanvasLayer/Level UI");
        Level = new LevelManager(linker.GetNode<Node>("Level"));
        Popups = new Popups(linker);
        Tokens = new Tokens();
        Net = new Net();

        // for making dev life easier
        menu.Hide();
        LevelUI.Show();
        Level.CurrentLevel = "Level A1";
        Level.LoadLevel();

        // TEST
        var ctsServer = Tokens.Create("server_running");
        var ctsClient = Tokens.Create("client_running");

        Net.StartServer(25565, 10, ctsServer);
        Net.StartClient("127.0.0.1", 25565, ctsClient);
    }

    public async Task Update() 
    {
        await Net.Update();
    }

    public static void LoadMap()
    {
        var mapScript = (Map)Prefabs.Map.Instance();
        map.CallDeferred("add_child", mapScript); // need to wait for the engine because we are dealing with areas with is physics related
        Audio.PlayMusic("map_grassy");
    }

    public static void DestroyMap() => map.QueueFreeChildren();
}
