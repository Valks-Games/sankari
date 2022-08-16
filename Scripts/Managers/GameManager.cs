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

namespace MarioLikeGame;

public class GameManager : Node
{
    [Export] protected readonly NodePath NodePathTransition;

    // managers
    public TransitionManager TransitionManager { get; private set; }
    public LevelManager LevelManager { get; private set; }
    public Audio Audio { get; private set; }

    private Node _map;

    public override void _Ready()
    {
        _map = GetNode<Node>("Map");

        Audio = new Audio(new GAudioStreamPlayer(this), new GAudioStreamPlayer(this));
        LevelManager = new LevelManager(this, GetNode<Node>("Level"));
        TransitionManager = GetNode<TransitionManager>(NodePathTransition);

        LoadMap();
    }

    public override void _Process(float delta)
    {
        Logger.Update();
    }

    public void LoadMap()
    {
        var mapScript = (Map)Prefabs.Map.Instance();
        mapScript.PreInit(this);
        _map.CallDeferred("add_child", mapScript); // need to wait for the engine because we are dealing with areas with is physics related
        Audio.PlayMusic("map_grassy");
    }

    public void DestroyMap() => _map.QueueFreeChildren();
}
