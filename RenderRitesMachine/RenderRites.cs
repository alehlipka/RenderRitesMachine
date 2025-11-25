using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine;

/// <summary>
/// Core class of the RenderRites engine. Provides the entry point for creating and controlling the rendering window.
/// </summary>
public sealed class RenderRites : IDisposable
{
    private static readonly Lazy<RenderRites> LazyMachine = new(() => new RenderRites());

    /// <summary>
    /// The single RenderRites instance (singleton).
    /// </summary>
    public static RenderRites Machine => LazyMachine.Value;

    /// <summary>
    /// Current rendering window. May be null until <see cref="RunWindow"/> is called.
    /// </summary>
    public Window? Window { get; private set; }

    /// <summary>
    /// Scene manager responsible for handling application scenes.
    /// </summary>
    public readonly SceneManager Scenes;

    /// <summary>
    /// Shared assets service used by every scene.
    /// </summary>
    public readonly IAssetsService AssetsService;

    /// <summary>
    /// Shared time service used by every scene.
    /// </summary>
    public readonly ITimeService TimeService;

    /// <summary>
    /// Shared render service used by every scene.
    /// </summary>
    public readonly IRenderService RenderService;

    /// <summary>
    /// Logger used to write informational messages and errors.
    /// </summary>
    public readonly ILogger Logger;

    /// <summary>
    /// Shared audio service used by every scene.
    /// </summary>
    public readonly IAudioService AudioService;

    /// <summary>
    /// GUI service that provides a software surface and input events.
    /// </summary>
    public readonly IGuiService GuiService;

    private RenderRites()
    {
        Logger = new Logger();
        AssetsService = new AssetsService(Logger);
        TimeService = new TimeService();
        RenderService = new RenderService();
        AudioService = new AudioService(Logger);
        GuiService = new GuiService(Logger);

        var sceneFactory = new SceneFactory(AssetsService, TimeService, RenderService, AudioService, GuiService, Logger);
        Scenes = new SceneManager(sceneFactory, Logger);

        sceneFactory.SetSceneManager(Scenes);
    }

    /// <summary>
    /// Overrides rendering settings for the current engine instance.
    /// </summary>
    /// <param name="settings">Complete user-provided configuration.</param>
    /// <returns>The same <see cref="RenderRites"/> instance to allow fluent configuration.</returns>
    public RenderRites ConfigureRenderSettings(RenderSettings settings)
    {
        RenderConstants.Configure(settings);
        return this;
    }

    /// <summary>
    /// Overrides rendering settings based on the current values.
    /// </summary>
    /// <param name="configure">Function that receives the current settings and returns a modified copy.</param>
    /// <returns>The same <see cref="RenderRites"/> instance to allow fluent configuration.</returns>
    public RenderRites ConfigureRenderSettings(Func<RenderSettings, RenderSettings> configure)
    {
        RenderConstants.Configure(configure);
        return this;
    }

    /// <summary>
    /// Creates and runs the rendering window using the provided or default title.
    /// </summary>
    /// <param name="title">Desired window title or null to use the default value.</param>
    public void RunWindow(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            title = "RenderRites Machine";
        }

        Scenes.Initialize();

        GameWindowSettings gws = new()
        {
            UpdateFrequency = RenderConstants.UpdateFrequency
        };

        NativeWindowSettings nws = new()
        {
            Title = title,
            ClientSize = new Vector2i(RenderConstants.DefaultWindowWidth, RenderConstants.DefaultWindowHeight),
            Profile = ContextProfile.Core,
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6, 0),
            Vsync = RenderConstants.DefaultVSyncMode,
            NumberOfSamples = RenderConstants.DefaultSamples,
            IsEventDriven = false,
            MinimumClientSize = new Vector2i(RenderConstants.MinWindowWidth, RenderConstants.MinWindowHeight),
            WindowState = RenderConstants.DefaultWindowState
        };

        nws.Flags |= ContextFlags.Debug;

        Logger.LogInfo($"Starting RenderRites window: '{title}' ({RenderConstants.DefaultWindowWidth}x{RenderConstants.DefaultWindowHeight})");
        Window = new Window(gws, nws, Scenes, GuiService, RenderService, Logger);
        Window.Run();
        Logger.LogInfo("Window closed, disposing resources");
        Scenes.Dispose();
        if (RenderService is IDisposable disposableRenderService)
        {
            disposableRenderService.Dispose();
        }
        AudioService.Dispose();
        AssetsService.Dispose();
        GuiService.Dispose();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
