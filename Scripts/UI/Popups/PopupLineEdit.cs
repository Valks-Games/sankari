namespace Sankari;

public partial class PopupLineEdit : Window
{
    [Export] protected NodePath NodePathLineEdit { get; set; }
    private LineEdit LineEdit { get; set; }

    private string PopupTitle { get; set; }
    private Popups PopupManager { get; set; }
    private Action<LineEdit> OnTextChanged { get; set; }
    private Action<string> OnHide { get; set; }
    private int MaxLength { get; set; }
    private string Text { get; set; }

    public void PreInit(Popups popupManager, Action<LineEdit> onTextChanged, Action<string> onHide, int maxLength = 50, string title = "", string text = "")
    {
        PopupManager = popupManager;
        PopupTitle = title;
        OnTextChanged = onTextChanged;
        OnHide = onHide;
        MaxLength = maxLength;
        Text = text;
    }

    public override void _Ready()
    {
        LineEdit = GetNode<LineEdit>(NodePathLineEdit);
        LineEdit.MaxLength = MaxLength;
        base.Title = PopupTitle;
        LineEdit.Text = Text;
    }

    private void _on_LineEdit_text_changed(string newText) => OnTextChanged(LineEdit);

    private void _on_PopupLineEdit_popup_hide()
    {
        OnHide(LineEdit.Text);
        PopupManager.Next();
        QueueFree();
    }

    private void _on_Ok_pressed() => Hide();
}
