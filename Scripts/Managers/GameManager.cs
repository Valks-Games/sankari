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

	// notifications
	public static EventManager<Event> Events {  get; private set; } = new();
	public static EventManager<EventPlayer> EventsPlayer { get; private set; } = new();

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
        Net.Init();
		PlayerManager = new PlayerManager(2, 6); //this numbers are for testing purposes!
		LevelUI.SetLabelLives(2); //required for the arbitrary lives count

        LevelUI.Hide();
    }

    public static async Task Update()
    {
        await Net.Update();
    }

    public static void ShowMenu() => Menu.Show();

    public static void LoadMap()
    {
        LevelUI.Show();

        // weird place to put this but its whatever right now
        Map = (Map)Prefabs.Map.Instantiate(); 

        NodeMap.CallDeferred("add_child", Map); // need to wait for the engine because we are dealing with areas with is physics related
        GameManager.Events.Notify(Event.OnMapLoaded);
    }

    public static void DestroyMap() => NodeMap.QueueFreeChildren();
}
