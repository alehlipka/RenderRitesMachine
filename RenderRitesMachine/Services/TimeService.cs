namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для управления временем между кадрами.
/// </summary>
public class TimeService : ITimeService
{
    public float UpdateDeltaTime { get; set; }
    public float RenderDeltaTime { get; set; }
}
