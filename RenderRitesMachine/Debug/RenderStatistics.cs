namespace RenderRitesMachine.Debug;

/// <summary>
///     Rendering statistics used for debugging and performance monitoring.
/// </summary>
public class RenderStatistics
{
    /// <summary>
    ///     Total number of objects processed for visibility.
    /// </summary>
    public int TotalObjects { get; set; }

    /// <summary>
    ///     Number of objects actually rendered.
    /// </summary>
    public int RenderedObjects { get; set; }

    /// <summary>
    ///     Resets statistics for a new frame.
    /// </summary>
    public void Reset()
    {
        TotalObjects = 0;
        RenderedObjects = 0;
    }
}
