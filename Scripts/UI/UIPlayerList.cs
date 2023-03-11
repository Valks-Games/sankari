namespace Sankari;

public partial class UIPlayerList : Control
{
    public Dictionary<byte, string> Players { get; set; } = new();
    private Control ControlPlayerList { get; set; }

    public override void _Ready()
    {
        ControlPlayerList = GetNode<Control>("VBox");
        Hide();
    }

    public void SetupListeners() 
    {
        GameManager.Events.AddListener(Event.OnGameClientLeft, (args) => 
        {
            RemovePlayer((byte)args[0]);
        });

        GameManager.Events.AddListener(Event.OnGameClientJoined, (args) => 
        {
            AddPlayer((byte)args[0], (string)args[1]);
        });

        GameManager.Events.AddListener(Event.OnReceivePlayersFromServer, (args) => 
        {
            var usernames = (Dictionary<byte, string>)args[0];

            foreach (var player in usernames)
                AddPlayer(player.Key, player.Value);
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
        foreach (Label child in ControlPlayerList.GetChildren())
            child.QueueFree();

        Players.Clear();

        Hide();
    }

    private void AddLabel(string text)
    {
        var label = new Label();
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.Text = text;
        label.Name = text;
        ControlPlayerList.AddChild(label);
    }

    private void RemoveLabel(string text)
    {
        foreach (Label child in ControlPlayerList.GetChildren())
        {
            if (child.Name == text) 
            {
                child.QueueFree();
                break;
            }
        }

        // have to wait a frame for the child count to update so that's why were checking if equal to 1 instead of 0
        if (ControlPlayerList.GetChildCount() == 1) 
            Hide();
    }
}
