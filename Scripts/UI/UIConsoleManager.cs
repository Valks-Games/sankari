namespace Sankari;

public partial class UIConsoleManager : Control
{
    public bool ScrollToBottom { get; set; } = true;

    private TextEdit ConsoleLogs { get; set; }
    private LineEdit ConsoleInput { get; set; }

    private Dictionary<int, string> CommandHistory { get; } = new();
    private int CommandHistoryIndex { get; set; }
    private int CommandHistoryNav { get; set; }

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

    public override void _Input(InputEvent @event)
    {
        if (!Visible || CommandHistory.Count == 0)
            return;

        if (Input.IsActionJustPressed("ui_up")) 
        {
            CommandHistoryNav--;

            if (!CommandHistory.ContainsKey(CommandHistoryNav)) 
                CommandHistoryNav++;

            ConsoleInput.Text = CommandHistory[CommandHistoryNav];

            // if deferred is not use then something else will override these settings
            ConsoleInput.CallDeferred("grab_focus");
            ConsoleInput.CallDeferred("set", "caret_position", CommandHistory[CommandHistoryNav].Length);
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            CommandHistoryNav++;

            if (!CommandHistory.ContainsKey(CommandHistoryNav)) 
                CommandHistoryNav--;

            ConsoleInput.Text = CommandHistory[CommandHistoryNav];

            // if deferred is not use then something else will override these settings
            ConsoleInput.CallDeferred("grab_focus");
            ConsoleInput.CallDeferred("set", "caret_position", CommandHistory[CommandHistoryNav].Length);
        }
    }

    private void _on_Console_Input_text_entered(string text)
    {
        var inputArr = text.Trim().ToLower().Split(' ');
        var cmd = inputArr[0];

        if (string.IsNullOrWhiteSpace(cmd))
            return;

        var command = Command.Instances.FirstOrDefault(x => x.IsMatch(cmd));

        if (command != null) 
        {
            var cmdArgs = inputArr.Skip(1).ToArray();

            command.Run(cmdArgs);
            CommandHistory.Add(CommandHistoryIndex++, $"{cmd}{(cmdArgs.Length == 0 ? "" : " ")}{string.Join(" ", cmdArgs)}");
            CommandHistoryNav = CommandHistoryIndex;
        }
        else
            Logger.Log($"The command '{cmd}' does not exist");

        ConsoleInput.Clear();
    }
}
