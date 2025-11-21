
namespace RenderRitesMachine.Services;

/// <summary>
/// Реализация системы логирования с выводом в консоль.
/// </summary>
public class Logger : ILogger
{
    private static readonly Lock _lockObject = new();

    /// <summary>
    /// Минимальный уровень логирования. Сообщения ниже этого уровня не будут выводиться.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

    /// <summary>
    /// Записывает сообщение уровня Debug.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    public void LogDebug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    /// <summary>
    /// Записывает сообщение уровня Info.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    public void LogInfo(string message)
    {
        Log(LogLevel.Info, message);
    }

    /// <summary>
    /// Записывает сообщение уровня Warning.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    public void LogWarning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    /// <summary>
    /// Записывает сообщение уровня Error.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    public void LogError(string message)
    {
        Log(LogLevel.Error, message);
    }

    /// <summary>
    /// Записывает сообщение уровня Critical.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    public void LogCritical(string message)
    {
        Log(LogLevel.Critical, message);
    }

    /// <summary>
    /// Записывает исключение с указанным уровнем логирования.
    /// </summary>
    /// <param name="level">Уровень логирования.</param>
    /// <param name="exception">Исключение для логирования.</param>
    /// <param name="message">Дополнительное сообщение. Может быть null.</param>
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
    /// Записывает сообщение с указанным уровнем логирования.
    /// </summary>
    /// <param name="level">Уровень логирования.</param>
    /// <param name="message">Сообщение для логирования.</param>
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
