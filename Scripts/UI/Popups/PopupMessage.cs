namespace Sankari;

public partial class PopupMessage : Window
{
    [Export] protected  NodePath NodePathMessage;

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
        Title = _title;
        GetNode<Label>(NodePathMessage).Text = _message;
    }

    private void _on_UIPopupMessage_popup_hide()
    {
        _popupManager.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
