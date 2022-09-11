namespace Sankari;

public class Popups
{
    private readonly Queue<WindowDialog> queue = new();
    private readonly Linker linker;

    public Popups(Linker linker)
    {
       this.linker = linker;
    }

    public void SpawnMessage(string message, string title = "")
    {
        var popup = Prefabs.PopupMessage.Instance<PopupMessage>();
        popup.PreInit(this, message, title);

        Spawn(popup);
    }

    public void SpawnError(Exception exception, string title = "")
    {
        var popup = Prefabs.PopupError.Instance<PopupError>();
        popup.PreInit(this, exception, title);

        Spawn(popup);
    }

    public void SpawnLineEdit(Action<LineEdit> onTextChanged, Action<string> onHide, string title = "", int maxLength = 50, string text = "")
    {
        var popup = Prefabs.PopupLineEdit.Instance<PopupLineEdit>();
        popup.PreInit(this, onTextChanged, onHide, maxLength, title, text);

        Spawn(popup);
    }

    /*public void SpawnCreateLobby()
    {
        var popup = Prefabs.PopupCreateLobby.Instance<PopupCreateLobby>();
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

    private void Spawn(WindowDialog popup)
    {
        linker.CanvasLayer.AddChild(popup);

        if (queue.Count == 0)
            popup.PopupCentered(popup.RectMinSize);

        queue.Enqueue(popup);
    }
}
