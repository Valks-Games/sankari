namespace Sankari;

public partial class PopupError : Window
{
    [Export] protected NodePath NodePathError { get; set; }

    private string Message { get; set; }
    private string PopupTitle { get; set; }
    private Popups PopupManager { get; set; }

    public void PreInit(Popups popupManager, Exception exception, string title = "")
    {
        PopupManager = popupManager;
        Message = exception.StackTrace;

        PopupTitle = !string.IsNullOrWhiteSpace(title) ? title : exception.Message;
    }

    public override void _Ready()
    {
        base.Title = PopupTitle;
        GetNode<TextEdit>(NodePathError).Text = Message;
    }

    private void _on_UIPopupError_popup_hide()
    {
        PopupManager.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
