namespace Sankari;

public partial class Popups
{
    private readonly Queue<Window> queue = new();
    private readonly Linker linker;

    public Popups(Linker linker)
    {
       this.linker = linker;
    }

    public void SpawnMessage(string message, string title = "")
    {
        var popup = Prefabs.PopupMessage.Instantiate<PopupMessage>();
        popup.PreInit(this, message, title);

        Spawn(popup);
    }

    public void SpawnError(Exception exception, string title = "")
    {
        var popup = Prefabs.PopupError.Instantiate<PopupError>();
        popup.PreInit(this, exception, title);

        Spawn(popup);
    }

    public void SpawnLineEdit(Action<LineEdit> onTextChanged, Action<string> onHide, string title = "", int maxLength = 50, string text = "")
    {
        var popup = Prefabs.PopupLineEdit.Instantiate<PopupLineEdit>();
        popup.PreInit(this, onTextChanged, onHide, maxLength, title, text);

        Spawn(popup);
    }

    /*public void SpawnCreateLobby()
    {
        var popup = Prefabs.PopupCreateLobby.Instantiate<PopupCreateLobby>();
        popup.PreInit(this);

        Spawn(popup);
    }*/

    public void Next()
    {
        queue.Dequeue();
        if (queue.Count == 0)
            return;
        var popup = queue.Peek();
        popup.PopupCentered();
    }

    private void Spawn(Window popup)
    {
        linker.CanvasLayer.AddChild(popup);

        if (queue.Count == 0)
            popup.PopupCentered(popup.MinSize);

        queue.Enqueue(popup);
    }
}
