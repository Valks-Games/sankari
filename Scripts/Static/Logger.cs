namespace Sankari;

public static class Logger
{
    private static readonly ConcurrentQueue<LogInfo> _messages = new();

    public static void Log(object message, ConsoleColor color = ConsoleColor.Gray) =>
        _messages.Enqueue(new LogInfo(LoggerOpcode.Message, new LogMessage($"{message}"), color));

    public static void LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow) =>
        Log($"[Warning] {message}", color);

    public static void LogTodo(object message, ConsoleColor color = ConsoleColor.White) =>
        Log($"[Todo] {message}", color);

    public static void LogErr(Exception e, string hint = "", ConsoleColor color = ConsoleColor.Red) =>
        _messages.Enqueue
            (new LogInfo(
                LoggerOpcode.Exception,
                new LogMessage($"[Error] {(string.IsNullOrWhiteSpace(hint) ? "" : $"'{hint}' ")}{e.Message}\n{e.StackTrace}"),
                color
            ));

    public static void LogDebug
    (
        object message,
        ConsoleColor color = ConsoleColor.Magenta,
        bool trace = true,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0
    ) =>
        _messages.Enqueue
        (new LogInfo(
            LoggerOpcode.Debug,
            new LogMessageTrace($"[Debug] {message}", trace, $"   at {filePath.Substring(filePath.IndexOf("Scripts\\"))} line:{lineNumber}"),
            color
        ));

    public static void LogMs(Action code)
    {
        var watch = new Stopwatch();
        watch.Start();
        code();
        watch.Stop();
        Log($"Took {watch.ElapsedMilliseconds} ms", ConsoleColor.DarkGray);
    }

    public static bool StillWorking() => _messages.Count > 0;

    public static void Update()
    {
        if (!_messages.TryDequeue(out LogInfo result))
            return;

        switch (result.Opcode)
        {
            case LoggerOpcode.Message:
            case LoggerOpcode.Exception:
                GameManager.Console.AddMessage(result.Data.Message);
                Print(result.Data.Message, result.Color);
                Console.ResetColor();
                break;

            case LoggerOpcode.Debug:
                var data = (LogMessageTrace)result.Data;
                GameManager.Console.AddMessage(data.Message);
                Print(data.Message, result.Color);
                if (data.Trace)
                {
                    Print(data.TracePath, ConsoleColor.DarkGray);
                }
                Console.ResetColor();
                break;
        }
    }

    private static void Print(object v, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        GD.Print(v);
    }

    private static void PrintErr(object v, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        GD.PrintErr(v);
    }
}

public class LogInfo
{
    public LoggerOpcode Opcode { get; set; }
    public LogMessage Data { get; set; }
    public ConsoleColor Color { get; set; }

    public LogInfo(LoggerOpcode opcode, LogMessage data, ConsoleColor color = ConsoleColor.Gray)
    {
        Opcode = opcode;
        Data = data;
        Color = color;
    }
}

public class LogMessage
{
    public string Message { get; set; }
    public LogMessage(string message) => this.Message = message;

}
public class LogMessageTrace : LogMessage
{
    public bool Trace { get; set; }
    public string TracePath { get; set; }

    public LogMessageTrace(string message, bool trace = true, string tracePath = default)
    : base(message)
    {
        Trace = trace;
        TracePath = tracePath;
    }
}

public enum LoggerOpcode
{
    Message,
    Exception,
    Debug
}