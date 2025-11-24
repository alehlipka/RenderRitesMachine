using OpenTK.Windowing.Common;

namespace RenderRitesMachine.Configuration;

/// <summary>
/// Global access point for render settings with override support.
/// </summary>
public static class RenderConstants
{
    /// <summary>
    /// Current engine settings.
    /// </summary>
    public static RenderSettings Settings { get; private set; } = new();

    /// <summary>
    /// Resets settings to their defaults.
    /// </summary>
    public static void Reset()
    {
        Settings = new RenderSettings();
    }

    /// <summary>
    /// Replaces the settings with a new instance.
    /// </summary>
    public static void Configure(RenderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Validate();
        Settings = settings with { };
    }

    /// <summary>
    /// Updates the settings based on the current values.
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
