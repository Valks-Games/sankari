namespace Sankari;

public class UIPlayerList : Control
{
    private Control controlPlayerList;

    public override void _Ready()
    {
        controlPlayerList = GetNode<Control>("VBox");
        Hide();
    }

    public void AddPlayer(string name)
    {
        AddLabel(name);
    }

    public void RemovePlayer(string name)
    {
        RemoveLabel(name);
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
        var label = controlPlayerList.FindNode(text);
        label.QueueFree();
    }
}
