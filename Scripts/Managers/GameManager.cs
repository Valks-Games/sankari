global using Godot;
global using GodotUtils;
global using System;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using System.Linq;

namespace Sankari;

public class GameManager
{
    public static Linker Linker { get; private set; }

    // managers
    public static TransitionManager Transition { get; private set; }
    public static LevelUIManager LevelUI { get; private set; }
    public static LevelScene LevelScene { get; set; } // odd ball
    public static UIPlayerList UIPlayerList { get; private set; }
    public static UIMapMenu UIMapMenu { get; private set; }
    public static Map Map { get; private set; }
    public static UIConsoleManager Console { get; private set; }
    public static UIMenu Menu { get; private set; }
    public static PlayerManager PlayerManager { get; private set; }

    private static Node NodeMap { get; set; }

    public GameManager(Linker linker) 
    {
        Linker = linker;
        NodeMap = linker.GetNode<Node>("Map");
        Menu = linker.GetNode<UIMenu>("CanvasLayer/Menu");
        
        Audio.Init(new GAudioStreamPlayer(linker), linker.SFXPlayers);

        Transition = linker.GetNode<TransitionManager>(linker.NodePathTransition);
        Console = linker.ConsoleManager;
        LevelUI = linker.GetNode<LevelUIManager>("CanvasLayer/Level UI");
        UIMapMenu = linker.UIMapMenu;
        UIPlayerList = linker.UIPlayerList;
        LevelManager.Init(linker.GetNode<Node>("Level"));
        Popups.Init(linker);
        PlayerManager = new PlayerManager(2, 6); //this numbers are for testing purposes!
        LevelUI.SetLabelLives(2); //required for the arbitrary lives count
        PlayerManager.SetLevelCoins();
        LevelUI.SetLabelCoins(PlayerManager.Coins);

        LevelUI.Hide();
    }

    public static void ShowMenu() => Menu.Show();

    public static void LoadMap()
    {
        LevelUI.Show();
        PlayerManager.SetLevelCoins();
        // weird place to put this but its whatever right now
        Map = (Map)Prefabs.Map.Instantiate(); 

        NodeMap.CallDeferred("add_child", Map); // need to wait for the engine because we are dealing with areas with is physics related
        Events.Generic.Notify(EventGeneric.OnMapLoaded);
    }

    public static void DestroyMap() => NodeMap.QueueFreeChildren();
}
