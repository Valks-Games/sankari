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

public class GameManager : Node
{
    [Export] protected readonly NodePath NodePathTransition;

    // managers
    public TransitionManager TransitionManager { get; private set; }
    public LevelManager LevelManager { get; private set; }
    public Audio Audio { get; private set; }

    private Node map;
    private UIMenu menu;

    public override void _Ready()
    {
        map = GetNode<Node>("Map");
        menu = GetNode<UIMenu>("CanvasLayer/Menu");
        menu.PreInit(this);

        Audio = new Audio(new GAudioStreamPlayer(this), new GAudioStreamPlayer(this));
        LevelManager = new LevelManager(this, GetNode<Node>("Level"));
        TransitionManager = GetNode<TransitionManager>(NodePathTransition);

        //Audio.PlayMusic("ice_1");

        // for making dev life easier
        menu.Hide();
        LevelManager.CurrentLevel = "Level A1";
        LevelManager.LoadLevel();
    }

    public override void _Process(float delta)
    {
        Logger.Update();
    }

    public void LoadMap()
    {
        var mapScript = (Map)Prefabs.Map.Instance();
        mapScript.PreInit(this);
        map.CallDeferred("add_child", mapScript); // need to wait for the engine because we are dealing with areas with is physics related
        Audio.PlayMusic("map_grassy");
    }

    public void DestroyMap() => map.QueueFreeChildren();
}
