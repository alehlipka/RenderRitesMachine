using OpenTK.Windowing.Common;

namespace RenderRitesMachine.Configuration;

/// <summary>
/// Глобальная точка доступа к настройкам рендеринга с поддержкой переопределения.
/// </summary>
public static class RenderConstants
{
    /// <summary>
    /// Текущие настройки движка.
    /// </summary>
    public static RenderSettings Settings { get; private set; } = new();

    /// <summary>
    /// Сбрасывает настройки к значениям по умолчанию.
    /// </summary>
    public static void Reset()
    {
        Settings = new RenderSettings();
    }

    /// <summary>
    /// Полностью переопределяет настройки.
    /// </summary>
    public static void Configure(RenderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Validate();
        Settings = settings with { };
    }

    /// <summary>
    /// Обновляет настройки на основе текущего состояния.
    /// </summary>
    public static void Configure(Func<RenderSettings, RenderSettings> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        Configure(configure(Settings));
    }

    public static int MaxStencilValue => Settings.MaxStencilValue;
    public static float AnisotropicFilteringLevel => Settings.AnisotropicFilteringLevel;
    public static float CameraNearPlane => Settings.CameraNearPlane;
    public static float CameraFarPlane => Settings.CameraFarPlane;
    public static float CameraMinFov => Settings.CameraMinFov;
    public static float CameraMaxFov => Settings.CameraMaxFov;
    public static float CameraMinPitch => Settings.CameraMinPitch;
    public static float CameraMaxPitch => Settings.CameraMaxPitch;
    public static int VertexAttributeSize => Settings.VertexAttributeSize;
    public static int PositionAttributeSize => Settings.PositionAttributeSize;
    public static float FloatEpsilon => Settings.FloatEpsilon;
    public static int MinWindowWidth => Settings.MinWindowWidth;
    public static int MinWindowHeight => Settings.MinWindowHeight;
    public static int DefaultWindowWidth => Settings.DefaultWindowWidth;
    public static int DefaultWindowHeight => Settings.DefaultWindowHeight;
    public static VSyncMode DefaultVSyncMode => Settings.DefaultVSyncMode;
    public static int DefaultSamples => Settings.DefaultSamples;
    public static WindowState DefaultWindowState => Settings.DefaultWindowState;
}
