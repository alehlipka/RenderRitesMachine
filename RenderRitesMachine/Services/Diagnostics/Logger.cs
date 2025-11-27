
namespace RenderRitesMachine.Services;

/// <summary>
/// Console-based logger implementation.
/// </summary>
public class Logger : ILogger
{
    private static readonly Lock _lockObject = new();

    /// <summary>
    /// Minimal logging level. Messages below this level are ignored.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

    /// <summary>
    /// Writes a debug-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogDebug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    /// <summary>
    /// Writes an info-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogInfo(string message)
    {
        Log(LogLevel.Info, message);
    }

    /// <summary>
    /// Writes a warning-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogWarning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    /// <summary>
    /// Writes an error-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogError(string message)
    {
        Log(LogLevel.Error, message);
    }

    /// <summary>
    /// Writes a critical-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    public void LogCritical(string message)
    {
        Log(LogLevel.Critical, message);
    }

    /// <summary>
    /// Logs an exception at the specified level.
    /// </summary>
    /// <param name="level">Logging level.</param>
    /// <param name="exception">Exception to log.</param>
    /// <param name="message">Optional additional message.</param>
    public void LogException(LogLevel level, Exception exception, string? message = null)
    {
        if (level < MinimumLevel)
        {
            return;
        }

        lock (_lockObject)
        {
            string logMessage = message != null
                ? $"{message}\n{exception}"
                : exception.ToString();

            WriteLog(level, logMessage);
        }
    }

    /// <summary>
    /// Logs a message with the specified level.
    /// </summary>
    /// <param name="level">Logging level.</param>
    /// <param name="message">Message to log.</param>
    public void Log(LogLevel level, string message)
    {
        if (level < MinimumLevel)
        {
            return;
        }

        lock (_lockObject)
        {
            WriteLog(level, message);
        }
    }


    private static void WriteLog(LogLevel level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string levelString = GetLevelString(level);
        ConsoleColor color = GetLevelColor(level);

        Console.ForegroundColor = color;
        Console.WriteLine($"[{timestamp}] [{levelString}] {message}");
        Console.ResetColor();
    }

    private static string GetLevelString(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO ",
            LogLevel.Warning => "WARN ",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRITICAL",
            _ => "UNKNOWN"
        };
    }

    private static ConsoleColor GetLevelColor(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.Magenta,
            _ => ConsoleColor.White
        };
    }
}
