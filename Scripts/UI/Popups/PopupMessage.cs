namespace Sankari;

public partial class PopupMessage : Window
{
    [Export] protected NodePath NodePathMessage { get; set; }

    private string Message { get; set; }
    private string PopupTitle { get; set; }
    private Popups PopupManager { get; set; }

    public void PreInit(Popups popupManager, string message, string title = "")
    {
        PopupManager = popupManager;
        Message = message;

        PopupTitle = !string.IsNullOrWhiteSpace(title) ? title : "";
    }

    public override void _Ready()
    {
        Title = PopupTitle;
        GetNode<Label>(NodePathMessage).Text = Message;
    }

    private void _on_UIPopupMessage_popup_hide()
    {
        PopupManager.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
