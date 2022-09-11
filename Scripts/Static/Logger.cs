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

    /// <summary>
    /// Logs an exception with trace information. Optionally allows logging a human readable hint
    /// </summary>
    public static void LogErr
    (
        Exception e,
        string hint = default,
        ConsoleColor color = ConsoleColor.Red,
        [CallerFilePath] string filePath = default,
        [CallerLineNumber] int lineNumber = 0
    ) => LogDetailed(LoggerOpcode.Exception, $@"[Error] {(string.IsNullOrWhiteSpace(hint) ? "" : $"'{hint}' ")}{e.Message}\n{e.StackTrace}", color, true, filePath, lineNumber);

    /// <summary>
    /// Logs a debug message that optionally contains trace information
    /// </summary>
    public static void LogDebug
    (
        object message,
        ConsoleColor color = ConsoleColor.Magenta,
        bool trace = true,
        [CallerFilePath] string filePath = default,
        [CallerLineNumber] int lineNumber = 0
    ) => LogDetailed(LoggerOpcode.Debug, $"[Debug] {message}", color, trace, filePath, lineNumber);

    public static void LogMs(Action code)
    {
        var watch = new Stopwatch();
        watch.Start();
        code();
        watch.Stop();
        Log($"Took {watch.ElapsedMilliseconds} ms", ConsoleColor.DarkGray);
    }

    public static bool StillWorking() => _messages.Count > 0;

    /// <summary>
    /// Dequeues a Requested Message and Logs it
    /// </summary>
    public static void Update()
    {
        if (!_messages.TryDequeue(out LogInfo result))
            return;

        switch (result.Opcode)
        {
            case LoggerOpcode.Message:
                GameManager.Console.AddMessage(result.Data.Message);
                Print(result.Data.Message, result.Color);
                Console.ResetColor();
                break;

            case LoggerOpcode.Exception:
                GameManager.Console.AddMessage(result.Data.Message);
                PrintErr(result.Data.Message, result.Color);
                if (result.Data is LogMessageTrace exceptionData)
                {
                    if (exceptionData.ShowTrace)
                    {
                        PrintErr(exceptionData.TracePath, ConsoleColor.DarkGray);
                    }
                }
                Console.ResetColor();
                break;

            case LoggerOpcode.Debug:
                GameManager.Console.AddMessage(result.Data.Message);
                Print(result.Data.Message, result.Color);
                if (result.Data is LogMessageTrace debugData)
                {
                    if (debugData.ShowTrace)
                    {
                        Print(debugData.TracePath, ConsoleColor.DarkGray);
                    }
                }
                Console.ResetColor();
                break;
        }
    }
    /// <summary>
    /// Logs a message that may contain trace information
    /// </summary>
    private static void LogDetailed(LoggerOpcode opcode, string message, ConsoleColor color, bool trace, string filePath, int lineNumber) =>
            _messages.Enqueue(new LogInfo(opcode, new LogMessageTrace(message, trace, $"  at {filePath.Substring(filePath.IndexOf("Scripts\\"))}:{lineNumber}"), color));

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
    // Show the Trace Information for the Message
    public bool ShowTrace { get; set; }
    public string TracePath { get; set; }

    public LogMessageTrace(string message, bool trace = true, string tracePath = default)
    : base(message)
    {
        ShowTrace = trace;
        TracePath = tracePath;
    }
}

public enum LoggerOpcode
{
    Message,
    Exception,
    Debug
}