namespace RenderRitesMachine.Services;

/// <summary>
/// Уровни логирования.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Отладочная информация для разработчиков.
    /// </summary>
    Debug,

    /// <summary>
    /// Общая информационная информация о работе приложения.
    /// </summary>
    Info,

    /// <summary>
    /// Предупреждения о потенциальных проблемах.
    /// </summary>
    Warning,

    /// <summary>
    /// Ошибки, которые не останавливают работу приложения.
    /// </summary>
    Error,

    /// <summary>
    /// Критические ошибки, которые могут привести к остановке приложения.
    /// </summary>
    Critical
}

/// <summary>
/// Интерфейс для системы логирования.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Минимальный уровень логирования. Сообщения ниже этого уровня не будут выводиться.
    /// </summary>
    LogLevel MinimumLevel { get; set; }

    /// <summary>
    /// Записывает сообщение уровня Debug.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    void LogDebug(string message);

    /// <summary>
    /// Записывает сообщение уровня Info.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    void LogInfo(string message);

    /// <summary>
    /// Записывает сообщение уровня Warning.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    void LogWarning(string message);

    /// <summary>
    /// Записывает сообщение уровня Error.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    void LogError(string message);

    /// <summary>
    /// Записывает сообщение уровня Critical.
    /// </summary>
    /// <param name="message">Сообщение для логирования.</param>
    void LogCritical(string message);

    /// <summary>
    /// Записывает исключение с указанным уровнем логирования.
    /// </summary>
    /// <param name="level">Уровень логирования.</param>
    /// <param name="exception">Исключение для логирования.</param>
    /// <param name="message">Дополнительное сообщение. Может быть null.</param>
    void LogException(LogLevel level, Exception exception, string? message = null);

    /// <summary>
    /// Записывает сообщение с указанным уровнем логирования.
    /// </summary>
    /// <param name="level">Уровень логирования.</param>
    /// <param name="message">Сообщение для логирования.</param>
    void Log(LogLevel level, string message);
}
