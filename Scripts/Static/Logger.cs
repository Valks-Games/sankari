namespace MarioLikeGame;

public static class Logger
{
    private static readonly ConcurrentQueue<LogInfo> _messages = new();

    public static void LogErr(Exception e, string hint = "", ConsoleColor c = ConsoleColor.Red) => _messages.Enqueue(new LogInfo(LoggerOpcode.Exception, $"[Error]: {(string.IsNullOrWhiteSpace(hint) ? "" : $"'{hint}' ")}{e.Message}\n{e.StackTrace}", c));
    public static void LogDebug(object v, ConsoleColor c = ConsoleColor.Magenta, bool trace = true, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) => _messages.Enqueue(new LogInfo(LoggerOpcode.Debug, new LogMessageDebug($"[Debug]: {v}", trace, $"   at {filePath.Substring(filePath.IndexOf("Scripts\\"))} line:{lineNumber}"), c));
    public static void LogTodo(object v, ConsoleColor c = ConsoleColor.White) => Log($"[Todo]: {v}", c);
    public static void LogWarning(object v, ConsoleColor c = ConsoleColor.Yellow) => Log($"[Warning]: {v}", c);
    public static void Log(object v, ConsoleColor c = ConsoleColor.Gray) => _messages.Enqueue(new LogInfo(LoggerOpcode.Message, $"{v}", c));
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
        if (_messages.TryDequeue(out LogInfo result))
        {
            switch (result.Opcode)
            {
                case LoggerOpcode.Message:
                    Print((string)result.Data, result.Color);
                    Console.ResetColor();
                    break;

                case LoggerOpcode.Exception:
                    PrintErr((string)result.Data, result.Color);
                    Console.ResetColor();
                    break;

                case LoggerOpcode.Debug:
                    var data = (LogMessageDebug)result.Data;
                    Print(data.Message, result.Color);
                    if (data.Trace)
                    {
                        Print(data.TracePath, ConsoleColor.DarkGray);
                    }
                    Console.ResetColor();
                    break;
            }
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
    public object Data { get; set; }
    public ConsoleColor Color { get; set; }

    public LogInfo(LoggerOpcode opcode, object data, ConsoleColor color = ConsoleColor.Gray)
    {
        Opcode = opcode;
        Data = data;
        Color = color;
    }
}

public class LogMessageDebug
{
    public string Message { get; set; }
    public bool Trace { get; set; }
    public string TracePath { get; set; }

    public LogMessageDebug(string message, bool trace = true, string tracePath = "")
    {
        Message = message;
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