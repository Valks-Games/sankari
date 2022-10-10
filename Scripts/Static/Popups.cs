namespace Sankari;

public static class Popups
{
    private static Queue<Window> Queue { get; } = new();
    private static Linker Linker { get; set; }

    public static void Init(Linker linker)
    {
       Linker = linker;
    }

    public static void SpawnMessage(string message, string title = "")
    {
        var popup = Prefabs.PopupMessage.Instantiate<PopupMessage>();
        popup.PreInit(message, title);

        Spawn(popup);
    }

    public static void SpawnError(Exception exception, string title = "")
    {
        var popup = Prefabs.PopupError.Instantiate<PopupError>();
        popup.PreInit(exception, title);

        Spawn(popup);
    }

    public static void SpawnLineEdit(Action<LineEdit> onTextChanged, Action<string> onHide, string title = "", int maxLength = 50, string text = "")
    {
        var popup = Prefabs.PopupLineEdit.Instantiate<PopupLineEdit>();
        popup.PreInit(onTextChanged, onHide, maxLength, title, text);

        Spawn(popup);
    }

    /*public void SpawnCreateLobby()
    {
        var popup = Prefabs.PopupCreateLobby.Instantiate<PopupCreateLobby>();
        popup.PreInit(this);

        Spawn(popup);
    }*/

    public static void Next()
    {
        Queue.Dequeue();
        if (Queue.Count == 0)
            return;
        var popup = Queue.Peek();
        popup.PopupCentered();
    }

    private static void Spawn(Window popup)
    {
        Linker.CanvasLayer.AddChild(popup);

        if (Queue.Count == 0)
            popup.PopupCentered(popup.MinSize);

        Queue.Enqueue(popup);
    }
}
