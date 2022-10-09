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

	// notifications
	public static Notifications<Event> Events {  get; private set; } = new();
	public static Notifications<PlayerEvent> PlayerEvents { get; private set; } = new();

    private static Node NodeMap { get; set; }

    public GameManager(Linker linker) 
    {
        Linker = linker;
        NodeMap = linker.GetNode<Node>("Map");
        Menu = linker.GetNode<UIMenu>("CanvasLayer/Menu");
        
        Audio.Init(new GAudioStreamPlayer(linker), new GAudioStreamPlayer(linker));
		LoadSoundEffects();
		LoadSoundTracks();

        Transition = linker.GetNode<TransitionManager>(linker.NodePathTransition);
        Console = linker.ConsoleManager;
        LevelUI = linker.GetNode<LevelUIManager>("CanvasLayer/Level UI");
        UIMapMenu = linker.UIMapMenu;
        UIPlayerList = linker.UIPlayerList;
        LevelManager.Init(linker.GetNode<Node>("Level"));
        Popups.Init(linker);
        Net.Init();

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
        Audio.PlayMusic("map_grassy");
    }

    public static void DestroyMap() => NodeMap.QueueFreeChildren();

	private static void LoadSoundEffects()
    {
        Audio.LoadSFX("player_jump", "Movement/Jump/sfx_movement_jump1.wav");
        Audio.LoadSFX("coin_pickup_1", "Environment/Coin Pickup/1/sfx_coin_single1.wav");
        Audio.LoadSFX("coin_pickup_2", "Environment/Coin Pickup/2/coin.wav");
        Audio.LoadSFX("dash", "Movement/Dash/swish-9.wav");

        Audio.LoadSFX("game_over_1", "Game Over/1/retro-game-over.wav");
        Audio.LoadSFX("game_over_2", "Game Over/2/game-over-dark-orchestra.wav");
        Audio.LoadSFX("game_over_3", "Game Over/3/musical-game-over.wav");
        Audio.LoadSFX("game_over_4", "Game Over/4/orchestra-game-over.wav");
    }

    private static void LoadSoundTracks()
    {
        Audio.LoadMusic("map_grassy", "Map/8bit Bossa/8bit Bossa.mp3");
        Audio.LoadMusic("grassy_1", "Level/Grassy Peaceful/Chiptune Adventures/Juhani Junkala [Chiptune Adventures] 1. Stage 1.ogg");
        Audio.LoadMusic("grassy_2", "Level/Grassy Peaceful/Chiptune Adventures/Juhani Junkala [Chiptune Adventures] 2. Stage 2.ogg");
        Audio.LoadMusic("ice_1", "Level/Ice/Icy_Expanse.mp3");
    }
}
