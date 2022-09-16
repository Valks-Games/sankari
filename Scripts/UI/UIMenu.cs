namespace Sankari;

public partial class UIMenu : Control
{
    private void _on_Play_pressed() 
    {
        PressPlay();
    }

    public void PressPlay()
    {
        Hide();
        GameManager.LoadMap();
    }

    //private async void _on_Options_pressed() => await _managers.ManagerScene.ChangeSceneToFile(GameScene.Options);
    //private async void _on_Mods_pressed() => await _managers.ManagerScene.ChangeSceneToFile(GameScene.Mods);
    //private async void _on_Credits_pressed() => await _managers.ManagerScene.ChangeSceneToFile(GameScene.Credits);
    private void _on_Quit_pressed() => GetTree().Notification((int)NotificationWmCloseRequest);
    private void _on_Discord_pressed() => OS.ShellOpen("https://discord.gg/5frafxrwwd");
    private void _on_GitHub_pressed() => OS.ShellOpen("https://github.com/Valks-Games/sankari");
}
