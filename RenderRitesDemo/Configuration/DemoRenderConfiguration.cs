using OpenTK.Windowing.Common;
using RenderRitesMachine.Configuration;

namespace RenderRitesDemo.Configuration;

/// <summary>
/// Demo-specific render settings.
/// </summary>
internal static class DemoRenderConfiguration
{
    public static RenderSettings Create()
    {
        return RenderConstants.Settings with
        {
            MinWindowWidth = 800,
            MinWindowHeight = 600,
            DefaultWindowWidth = 800,
            DefaultWindowHeight = 600,
            DefaultWindowState = WindowState.Normal,
            DefaultVSyncMode = VSyncMode.Off,
            DefaultSamples = 8
        };
    }
}
