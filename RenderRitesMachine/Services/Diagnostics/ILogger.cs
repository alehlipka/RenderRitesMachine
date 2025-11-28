namespace RenderRitesMachine.Services.Diagnostics;

/// <summary>
///     Logging levels.
/// </summary>
public enum LogLevel
{
    /// <summary>
    ///     Diagnostic information for developers.
    /// </summary>
    Debug,

    /// <summary>
    ///     General information about normal application behavior.
    /// </summary>
    Info,

    /// <summary>
    ///     Warnings about potential problems.
    /// </summary>
    Warning,

    /// <summary>
    ///     Errors that do not stop application execution.
    /// </summary>
    Error,

    /// <summary>
    ///     Critical errors that can stop the application.
    /// </summary>
    Critical
}

/// <summary>
///     Logging interface.
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     Minimal logging level. Messages below this level are ignored.
    /// </summary>
    LogLevel MinimumLevel { get; set; }

    /// <summary>
    ///     Writes a debug-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void LogDebug(string message);

    /// <summary>
    ///     Writes an info-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void LogInfo(string message);

    /// <summary>
    ///     Writes a warning-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void LogWarning(string message);

    /// <summary>
    ///     Writes an error-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void LogError(string message);

    /// <summary>
    ///     Writes a critical-level message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    void LogCritical(string message);

    /// <summary>
    ///     Logs an exception at the specified level.
    /// </summary>
    /// <param name="level">Logging level.</param>
    /// <param name="exception">Exception to log.</param>
    /// <param name="message">Optional additional message.</param>
    void LogException(LogLevel level, Exception exception, string? message = null);

    /// <summary>
    ///     Writes a message with the specified level.
    /// </summary>
    /// <param name="level">Logging level.</param>
    /// <param name="message">Message to log.</param>
    void Log(LogLevel level, string message);
}
