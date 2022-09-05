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

namespace Sankari;

public class GameManager
{
    // managers
    public static TransitionManager TransitionManager { get; private set; }
    public static LevelManager LevelManager { get; private set; }
    public static LevelUIManager LevelUIManager { get; private set; }
    public static Audio Audio { get; private set; }

    private static Node map;
    private static UIMenu menu;

    public GameManager(Linker linker) 
    {
        map = linker.GetNode<Node>("Map");
        menu = linker.GetNode<UIMenu>("CanvasLayer/Menu");
        
        Audio = new Audio(new GAudioStreamPlayer(linker), new GAudioStreamPlayer(linker));
        TransitionManager = linker.GetNode<TransitionManager>(linker.NodePathTransition);
        LevelUIManager = linker.GetNode<LevelUIManager>("CanvasLayer/Level UI");
        LevelManager = new LevelManager(linker.GetNode<Node>("Level"));

        // for making dev life easier
        menu.Hide();
        LevelUIManager.Show();
        LevelManager.CurrentLevel = "Level A1";
        LevelManager.LoadLevel();
    }

    public static void LoadMap()
    {
        var mapScript = (Map)Prefabs.Map.Instance();
        map.CallDeferred("add_child", mapScript); // need to wait for the engine because we are dealing with areas with is physics related
        Audio.PlayMusic("map_grassy");
    }

    public static void DestroyMap() => map.QueueFreeChildren();
}
