using OpenTK.Windowing.Common;

namespace RenderRitesMachine.Configuration;

/// <summary>
/// Пользовательские настройки движка с валидацией значений.
/// </summary>
public sealed record RenderSettings
{
    public int MaxStencilValue { get; init; } = 255;
    public float AnisotropicFilteringLevel { get; init; } = 8.0f;
    public float CameraNearPlane { get; init; } = 0.01f;
    public float CameraFarPlane { get; init; } = 1000.0f;
    public float CameraMinFov { get; init; } = 1.0f;
    public float CameraMaxFov { get; init; } = 90.0f;
    public float CameraMinPitch { get; init; } = -89.0f;
    public float CameraMaxPitch { get; init; } = 89.0f;
    public int VertexAttributeSize { get; init; } = 8;
    public int PositionAttributeSize { get; init; } = 3;
    public float FloatEpsilon { get; init; } = 0.0001f;
    public int MinWindowWidth { get; init; } = 800;
    public int MinWindowHeight { get; init; } = 600;
    public int DefaultWindowWidth { get; init; } = 800;
    public int DefaultWindowHeight { get; init; } = 600;
    public VSyncMode DefaultVSyncMode { get; init; } = VSyncMode.Off;
    public int DefaultSamples { get; init; } = 4;
    public WindowState DefaultWindowState { get; init; } = WindowState.Normal;

    /// <summary>
    /// Проверяет, что все значения лежат в допустимых пределах.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Если найдено некорректное значение.</exception>
    public void Validate()
    {
        if (MaxStencilValue is < 1 or > 255)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxStencilValue), MaxStencilValue, "Stencil value must be in range [1,255].");
        }

        if (AnisotropicFilteringLevel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(AnisotropicFilteringLevel), AnisotropicFilteringLevel, "Anisotropic filtering level must be positive.");
        }

        if (CameraNearPlane <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(CameraNearPlane), CameraNearPlane, "Near plane must be greater than zero.");
        }

        if (CameraFarPlane <= CameraNearPlane)
        {
            throw new ArgumentOutOfRangeException(nameof(CameraFarPlane), CameraFarPlane, "Far plane must be greater than near plane.");
        }

        if (CameraMinFov <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(CameraMinFov), CameraMinFov, "Min FOV must be positive.");
        }

        if (CameraMaxFov <= CameraMinFov || CameraMaxFov > 90.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(CameraMaxFov), CameraMaxFov, "Max FOV must be > Min FOV and <= 90.");
        }

        if (CameraMinPitch < -90.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(CameraMinPitch), CameraMinPitch, "Min pitch must be >= -90.");
        }

        if (CameraMaxPitch > 90.0f || CameraMaxPitch <= CameraMinPitch)
        {
            throw new ArgumentOutOfRangeException(nameof(CameraMaxPitch), CameraMaxPitch, "Max pitch must be <= 90 and greater than min.");
        }

        if (VertexAttributeSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(VertexAttributeSize), VertexAttributeSize, "Vertex attribute size must be positive.");
        }

        if (PositionAttributeSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(PositionAttributeSize), PositionAttributeSize, "Position attribute size must be positive.");
        }

        if (VertexAttributeSize < PositionAttributeSize)
        {
            throw new ArgumentOutOfRangeException(nameof(VertexAttributeSize), VertexAttributeSize, "Vertex attribute size must be >= position attribute size.");
        }

        if (FloatEpsilon is <= 0 or >= 0.001f)
        {
            throw new ArgumentOutOfRangeException(nameof(FloatEpsilon), FloatEpsilon, "Float epsilon must be in range (0, 0.001).");
        }

        if (MinWindowWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MinWindowWidth), MinWindowWidth, "Min window width must be positive.");
        }

        if (MinWindowHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MinWindowHeight), MinWindowHeight, "Min window height must be positive.");
        }

        if (DefaultWindowWidth < MinWindowWidth)
        {
            throw new ArgumentOutOfRangeException(nameof(DefaultWindowWidth), DefaultWindowWidth, "Default width must be >= minimal width.");
        }

        if (DefaultWindowHeight < MinWindowHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(DefaultWindowHeight), DefaultWindowHeight, "Default height must be >= minimal height.");
        }

        if (DefaultSamples is < 1 or > 16)
        {
            throw new ArgumentOutOfRangeException(nameof(DefaultSamples), DefaultSamples, "Samples must be in range [1,16].");
        }
    }
}
