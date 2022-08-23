using Godot;
using System;

namespace Sankari;

public class UIMenu : Control
{
    private GameManager _gameManager;

    public void PreInit(GameManager gameManager) => _gameManager = gameManager;

    private void _on_Play_pressed() 
    {
        Hide();
        _gameManager.LoadMap();
    }
    //private async void _on_Options_pressed() => await _managers.ManagerScene.ChangeScene(GameScene.Options);
    //private async void _on_Mods_pressed() => await _managers.ManagerScene.ChangeScene(GameScene.Mods);
    //private async void _on_Credits_pressed() => await _managers.ManagerScene.ChangeScene(GameScene.Credits);
    private void _on_Quit_pressed() => GetTree().Notification(MainLoop.NotificationWmQuitRequest);
    private void _on_Discord_pressed() => OS.ShellOpen("https://discord.gg/5frafxrwwd");
    private void _on_GitHub_pressed() => OS.ShellOpen("https://github.com/Valks-Games/sankari");
}
