namespace RenderRitesMachine.Services;

/// <summary>
/// Tracks delta times between update and render frames.
/// </summary>
public class TimeService : ITimeService
{
    public float UpdateDeltaTime { get; set; }
    public float RenderDeltaTime { get; set; }
}
