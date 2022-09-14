namespace Sankari;

public class UIConsoleManager : Control
{
    public bool ScrollToBottom { get; set; } = true;

    private TextEdit ConsoleLogs;
    private LineEdit ConsoleInput;

    private readonly Dictionary<int, string> commandHistory = new();
    private int commandHistoryIndex;
    private int commandHistoryNav;

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
        if (!Visible || commandHistory.Count == 0)
            return;

        if (Input.IsActionJustPressed("ui_up")) 
        {
            commandHistoryNav--;

            if (!commandHistory.ContainsKey(commandHistoryNav)) 
                commandHistoryNav++;

            ConsoleInput.Text = commandHistory[commandHistoryNav];

            // if deferred is not use then something else will override these settings
            ConsoleInput.CallDeferred("grab_focus");
            ConsoleInput.CallDeferred("set", "caret_position", commandHistory[commandHistoryNav].Length);
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            commandHistoryNav++;

            if (!commandHistory.ContainsKey(commandHistoryNav)) 
                commandHistoryNav--;

            ConsoleInput.Text = commandHistory[commandHistoryNav];

            // if deferred is not use then something else will override these settings
            ConsoleInput.CallDeferred("grab_focus");
            ConsoleInput.CallDeferred("set", "caret_position", commandHistory[commandHistoryNav].Length);
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
            commandHistory.Add(commandHistoryIndex++, $"{cmd}{(cmdArgs.Length == 0 ? "" : " ")}{string.Join(" ", cmdArgs)}");
            commandHistoryNav = commandHistoryIndex;
        }
        else
            Logger.Log($"The command '{cmd}' does not exist");

        ConsoleInput.Clear();
    }
}
