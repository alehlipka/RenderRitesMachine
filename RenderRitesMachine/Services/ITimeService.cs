namespace RenderRitesMachine.Services;

/// <summary>
/// Интерфейс для сервиса времени, предоставляющего информацию о времени между кадрами.
/// </summary>
public interface ITimeService
{
    /// <summary>
    /// Время между кадрами обновления.
    /// </summary>
    float UpdateDeltaTime { get; set; }

    /// <summary>
    /// Время между кадрами рендеринга.
    /// </summary>
    float RenderDeltaTime { get; set; }
}

