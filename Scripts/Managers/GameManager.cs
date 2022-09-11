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
global using Sankari.Netcode;
global using Sankari.Netcode.Client;
global using Sankari.Netcode.Server;

namespace Sankari;

public class GameManager
{
    public static Node Linker { get; private set; }

    // managers
    public static TransitionManager Transition { get; private set; }
    public static LevelManager Level { get; private set; }
    public static LevelUIManager LevelUI { get; private set; }
    public static LevelScene LevelScene { get; set; }
    public static UIPlayerList UIPlayerList { get; private set; }
    public static UIMapMenu UIMapMenu { get; private set; }
    public static Map Map { get; private set; }
    public static ConsoleManager Console { get; private set; }
    public static Audio Audio { get; private set; }
    public static Popups Popups { get; private set; }
    public static Tokens Tokens { get; private set; }
    public static Net Net { get; private set; }
    public static Notifications Notifications { get; private set; }

    private static Node map;
    private static UIMenu menu;

    public GameManager(Linker linker) 
    {
        Linker = linker;
        map = linker.GetNode<Node>("Map");
        menu = linker.GetNode<UIMenu>("CanvasLayer/Menu");
        
        Audio = new Audio(new GAudioStreamPlayer(linker), new GAudioStreamPlayer(linker));
        Transition = linker.GetNode<TransitionManager>(linker.NodePathTransition);
        Console = linker.ConsoleManager;
        LevelUI = linker.GetNode<LevelUIManager>("CanvasLayer/Level UI");
        UIMapMenu = linker.UIMapMenu;
        UIPlayerList = linker.UIPlayerList;
        Level = new LevelManager(linker.GetNode<Node>("Level"));
        Popups = new Popups(linker);
        Tokens = new Tokens();
        Notifications = new Notifications();
        Net = new Net();

        LevelUI.Hide();
        
    }

    public async Task Update()
    {
        await Net.Update();
    }

    public static void ShowMenu() => menu.Show();

    public static void LoadMap()
    {
        LevelUI.Show();

        // weird place to put this but its whatever right now
        Map = (Map)Prefabs.Map.Instance(); 

        map.CallDeferred("add_child", Map); // need to wait for the engine because we are dealing with areas with is physics related
        Audio.PlayMusic("map_grassy");
    }

    public static void DestroyMap() => map.QueueFreeChildren();
}
