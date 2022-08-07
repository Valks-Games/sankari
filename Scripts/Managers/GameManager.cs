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
    [Export] protected readonly NodePath NodePathMap;
    [Export] protected readonly NodePath NodePathLevel;

    private Node _map;
    public LevelManager LevelManager { get; private set; }

    public override void _Ready()
    {
        _map = GetNode<Node>(NodePathMap);

        LevelManager = new(this, GetNode<Node>(NodePathLevel));

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
    }

    public void DestroyMap() => _map.QueueFreeChildren();
}
