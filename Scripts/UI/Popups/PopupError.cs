namespace Sankari;

public class PopupError : WindowDialog
{
    [Export] protected readonly NodePath NodePathError;

    private string _message;
    private string _title;
    private Popups _popupManager;

    public void PreInit(Popups popupManager, Exception exception, string title = "")
    {
        _popupManager = popupManager;
        _message = exception.StackTrace;

        _title = !string.IsNullOrWhiteSpace(title) ? title : exception.Message;
    }

    public override void _Ready()
    {
        WindowTitle = _title;
        GetNode<TextEdit>(NodePathError).Text = _message;
    }

    private void _on_UIPopupError_popup_hide()
    {
        _popupManager.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
