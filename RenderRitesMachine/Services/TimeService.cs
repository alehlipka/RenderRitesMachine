namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для управления временем между кадрами.
/// </summary>
public class TimeService : ITimeService
{
    public float UpdateDeltaTime { get; set; } = 0.0f;
    public float RenderDeltaTime { get; set; } = 0.0f;
}
