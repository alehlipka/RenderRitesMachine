namespace RenderRitesMachine.Debug;

/// <summary>
/// Статистика рендеринга для отладки и мониторинга производительности.
/// </summary>
public class RenderStatistics
{
    /// <summary>
    /// Общее количество объектов, прошедших проверку видимости.
    /// </summary>
    public int TotalObjects { get; set; }

    /// <summary>
    /// Количество объектов, отсеченных frustum culling.
    /// </summary>
    public int CulledObjects { get; set; }

    /// <summary>
    /// Количество объектов, которые были отрендерены.
    /// </summary>
    public int RenderedObjects { get; set; }

    /// <summary>
    /// Процент объектов, отсеченных frustum culling.
    /// </summary>
    public float CullingPercentage => TotalObjects > 0 ? (CulledObjects / (float)TotalObjects) * 100.0f : 0.0f;

    /// <summary>
    /// Сбрасывает статистику для нового кадра.
    /// </summary>
    public void Reset()
    {
        TotalObjects = 0;
        CulledObjects = 0;
        RenderedObjects = 0;
    }
}

