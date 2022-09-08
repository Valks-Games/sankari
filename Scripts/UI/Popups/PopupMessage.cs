namespace Sankari;

public class PopupMessage : WindowDialog
{
    [Export] protected readonly NodePath NodePathMessage;

    private string _message;
    private string _title;
    private Popups _popupManager;

    public void PreInit(Popups popupManager, string message, string title = "")
    {
        _popupManager = popupManager;
        _message = message;

        _title = !string.IsNullOrWhiteSpace(title) ? title : "";
    }

    public override void _Ready()
    {
        WindowTitle = _title;
        GetNode<Label>(NodePathMessage).Text = _message;
    }

    private void _on_UIPopupMessage_popup_hide()
    {
        _popupManager.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
