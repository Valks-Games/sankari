namespace Sankari;

public class UIConsoleManager : Control
{
    public bool ScrollToBottom { get; set; }

    private TextEdit ConsoleLogs;
    private LineEdit ConsoleInput;

    public override void _Ready()
    {
        ConsoleLogs = GetNode<TextEdit>("Console Logs");
        ConsoleInput = GetNode<LineEdit>("Console Input");
    }

    public void AddException(Exception e) =>
        AddMessage($"{e.Message}\n{e.StackTrace}");

    public void AddMessage(string message)
    {
        ConsoleLogs.Text += $"{message}\n";
        ScrollDown();
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
        ConsoleInput.GrabFocus();
        ScrollDown();
    }

    private void ScrollDown() 
    {
        if (ScrollToBottom)
            ConsoleLogs.ScrollVertical = Mathf.Inf;
    }

    private void _on_Console_Input_text_entered(string text)
    {
        var inputArr = text.Trim().ToLower().Split(' ');
        var cmd = inputArr[0];

        if (string.IsNullOrWhiteSpace(cmd))
            return;

        var command = Command.Instances.FirstOrDefault(x => x.IsMatch(cmd));

        if (command != null)
            command.Run(inputArr.Skip(1).ToArray());
        else
            Logger.Log($"The command '{cmd}' does not exist");

        ConsoleInput.Clear();
    }
}
