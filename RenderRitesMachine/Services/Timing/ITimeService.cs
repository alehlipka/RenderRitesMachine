namespace RenderRitesMachine.Services;

/// <summary>
/// Time service interface that exposes per-frame delta values.
/// </summary>
public interface ITimeService
{
    /// <summary>
    /// Delta time between update frames.
    /// </summary>
    float UpdateDeltaTime { get; set; }

    /// <summary>
    /// Delta time between render frames.
    /// </summary>
    float RenderDeltaTime { get; set; }
}
