namespace Sankari;

public partial class PopupError : Window
{
    [Export] protected NodePath NodePathError { get; set; }

    private string Message { get; set; }
    private string PopupTitle { get; set; }

    public void PreInit(Exception exception, string title = "")
    {
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
        Popups.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
