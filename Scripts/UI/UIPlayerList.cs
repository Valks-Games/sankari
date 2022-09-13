namespace Sankari;

public class UIPlayerList : Control
{
    public Dictionary<byte, string> Players = new();
    private Control controlPlayerList;

    public override void _Ready()
    {
        controlPlayerList = GetNode<Control>("VBox");
        Hide();
    }

    public void SetupListeners() 
    {
        GameManager.Notifications.AddListener(this, Event.OnGameClientLeft, (args) => 
        {
            RemovePlayer((byte)args[0]);
        });

        GameManager.Notifications.AddListener(this, Event.OnGameClientJoined, (args) => 
        {
            AddPlayer((byte)args[0], (string)args[1]);
        });
    }

    public void AddPlayer(byte id, string name)
    {
        AddLabel(name);
        Players.Add(id, name);
        Show();
    }

    public void RemovePlayer(byte id)
    {
        RemoveLabel(Players[id]);
        Players.Remove(id);
    }

    public void RemoveAllPlayers()
    {
        foreach (Label child in controlPlayerList.GetChildren())
            child.QueueFree();

        Players.Clear();

        Hide();
    }

    private void AddLabel(string text)
    {
        var label = new Label();
        label.Align = Label.AlignEnum.Center;
        label.Text = text;
        label.Name = text;
        controlPlayerList.AddChild(label);
    }

    private void RemoveLabel(string text)
    {
        foreach (Label child in controlPlayerList.GetChildren())
        {
            if (child.Name == text) 
            {
                child.QueueFree();
                break;
            }
        }

        // have to wait a frame for the child count to update so that's why were checking if equal to 1 instead of 0
        if (controlPlayerList.GetChildCount() == 1) 
            Hide();
    }
}
