namespace RenderRitesMachine.Configuration;

/// <summary>
/// Константы для рендеринга и конфигурации движка.
/// </summary>
public static class RenderConstants
{
    /// <summary>
    /// Максимальное значение для stencil buffer (0-255).
    /// </summary>
    public const int MaxStencilValue = 255;
    
    /// <summary>
    /// Уровень анизотропной фильтрации текстур.
    /// </summary>
    public const float AnisotropicFilteringLevel = 8.0f;
    
    /// <summary>
    /// Ближняя плоскость отсечения камеры.
    /// </summary>
    public const float CameraNearPlane = 0.01f;
    
    /// <summary>
    /// Дальняя плоскость отсечения камеры.
    /// </summary>
    public const float CameraFarPlane = 1000.0f;
    
    /// <summary>
    /// Минимальный FOV камеры в градусах.
    /// </summary>
    public const float CameraMinFov = 1.0f;
    
    /// <summary>
    /// Максимальный FOV камеры в градусах.
    /// </summary>
    public const float CameraMaxFov = 90.0f;
    
    /// <summary>
    /// Минимальный pitch камеры в градусах.
    /// </summary>
    public const float CameraMinPitch = -89.0f;
    
    /// <summary>
    /// Максимальный pitch камеры в градусах.
    /// </summary>
    public const float CameraMaxPitch = 89.0f;
    
    /// <summary>
    /// Размер вершинного атрибута (position + normal + texture = 8 floats).
    /// </summary>
    public const int VertexAttributeSize = 8;
    
    /// <summary>
    /// Размер позиционного атрибута (position = 3 floats).
    /// </summary>
    public const int PositionAttributeSize = 3;
    
    /// <summary>
    /// Эпсилон для сравнения float значений.
    /// </summary>
    public const float FloatEpsilon = 0.0001f;
    
    /// <summary>
    /// Минимальный размер окна по ширине.
    /// </summary>
    public const int MinWindowWidth = 800;
    
    /// <summary>
    /// Минимальный размер окна по высоте.
    /// </summary>
    public const int MinWindowHeight = 600;
    
    /// <summary>
    /// Размер окна по умолчанию (ширина).
    /// </summary>
    public const int DefaultWindowWidth = 1024;
    
    /// <summary>
    /// Размер окна по умолчанию (высота).
    /// </summary>
    public const int DefaultWindowHeight = 768;
}

