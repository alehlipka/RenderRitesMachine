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
    /// Количество объектов, которые были отрендерены.
    /// </summary>
    public int RenderedObjects { get; set; }

    /// <summary>
    /// Сбрасывает статистику для нового кадра.
    /// </summary>
    public void Reset()
    {
        TotalObjects = 0;
        RenderedObjects = 0;
    }
}
