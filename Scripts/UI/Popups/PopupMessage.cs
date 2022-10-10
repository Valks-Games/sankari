namespace Sankari;

public partial class PopupMessage : Window
{
    [Export] protected NodePath NodePathMessage { get; set; }

    private string Message { get; set; }
    private string PopupTitle { get; set; }

    public void PreInit(string message, string title = "")
    {
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
        Popups.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
