namespace Sankari;

public class Popups
{
    private Queue<Window> Queue { get; } = new();
    private Linker Linker { get; }

    public Popups(Linker linker)
    {
       Linker = linker;
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
        Queue.Dequeue();
        if (Queue.Count == 0)
            return;
        var popup = Queue.Peek();
        popup.PopupCentered();
    }

    private void Spawn(Window popup)
    {
        Linker.CanvasLayer.AddChild(popup);

        if (Queue.Count == 0)
            popup.PopupCentered(popup.MinSize);

        Queue.Enqueue(popup);
    }
}
