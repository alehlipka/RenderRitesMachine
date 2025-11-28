using Leopotam.EcsLite;
using OpenTK.Windowing.Common;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services.Audio;
using RenderRitesMachine.Services.Diagnostics;
using RenderRitesMachine.Services.Graphics;
using RenderRitesMachine.Services.Gui;
using RenderRitesMachine.Services.Timing;

namespace RenderRitesMachine.Output;

/// <summary>
///     Base class for all application scenes. Provides an ECS world, update/render/resize systems,
///     a camera, and shared services.
/// </summary>
public abstract class Scene : IDisposable
{
    private readonly SystemSharedObject _shared;
    private readonly ITimeService _timeService;

    /// <summary>
    ///     Assets service that loads meshes, shaders, and textures.
    /// </summary>
    protected readonly IAssetsService Assets;

    /// <summary>
    ///     Audio service shared across the scene.
    /// </summary>
    protected readonly IAudioService Audio;

    /// <summary>
    ///     Scene camera responsible for view/projection transforms.
    /// </summary>
    protected readonly ICamera Camera;

    /// <summary>
    ///     Systems executed during the render phase each frame.
    /// </summary>
    protected readonly EcsSystems RenderSystems;

    /// <summary>
    ///     Systems executed when the window is resized.
    /// </summary>
    protected readonly EcsSystems ResizeSystems;

    /// <summary>
    ///     Systems executed during the update phase each frame.
    /// </summary>
    protected readonly EcsSystems UpdateSystems;

    /// <summary>
    ///     ECS world that stores entities and components.
    /// </summary>
    protected readonly EcsWorld World;

    private bool _isLoaded;

    protected Scene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService,
        IAudioService audioService, IGuiService guiService, ISceneManager sceneManager, ILogger logger)
        : this(name, assetsService, timeService, renderService, audioService, guiService, sceneManager, logger,
            new PerspectiveCamera())
    {
    }

    protected Scene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService,
        IAudioService audioService, IGuiService guiService, ISceneManager sceneManager, ILogger logger, ICamera camera)
    {
        ArgumentNullException.ThrowIfNull(camera);

        _timeService = timeService;
        Gui = guiService;
        Camera = camera;
        Assets = assetsService;
        Audio = audioService;
        _shared = new SystemSharedObject(Camera, _timeService, Assets, renderService, audioService, guiService,
            sceneManager, logger);

        Name = name;
        World = new EcsWorld();
        UpdateSystems = new EcsSystems(World, _shared);
        RenderSystems = new EcsSystems(World, _shared);
        ResizeSystems = new EcsSystems(World, _shared);
    }

    /// <summary>
    ///     Scene name.
    /// </summary>
    public string Name { get; }

    protected IGuiService Gui { get; }

    public void Dispose()
    {
        if (!_isLoaded)
        {
            return;
        }

        _isLoaded = false;

        try
        {
            ResizeSystems?.Destroy();
            UpdateSystems?.Destroy();
            RenderSystems?.Destroy();
            World?.Destroy();
        }
        catch (Exception ex)
        {
            _shared.Logger.LogException(LogLevel.Error, ex, $"Error disposing scene '{Name}'");
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    ///     Assigns the window instance so systems can access it via <see cref="SystemSharedObject" />.
    /// </summary>
    /// <param name="window">Application window.</param>
    public void SetWindow(Window window) => _shared.Window = window;

    /// <summary>
    ///     Initializes the scene. Called automatically on the first use.
    /// </summary>
    public void Initialize()
    {
        if (_isLoaded)
        {
            return;
        }

        _isLoaded = true;
        _shared.Logger.LogDebug($"Initializing scene '{Name}'");
        OnLoad();
        UpdateSystems.Init();
        ResizeSystems.Init();
        _shared.Logger.LogInfo($"Scene '{Name}' initialized successfully");
    }

    /// <summary>
    ///     Updates the scene each frame before rendering.
    /// </summary>
    /// <param name="args">Frame timing arguments.</param>
    public void UpdateScene(FrameEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        _timeService.UpdateDeltaTime = (float)args.Time;
        UpdateSystems.Run();
    }

    /// <summary>
    ///     Renders the scene each frame.
    /// </summary>
    /// <param name="args">Frame timing arguments.</param>
    public void RenderScene(FrameEventArgs args)
    {
        if (!_isLoaded)
        {
            return;
        }

        _timeService.RenderDeltaTime = (float)args.Time;
        _shared.ClearActiveShaders();
        RenderSystems.Run();
        _shared.UpdateActiveShaders();
    }

    /// <summary>
    ///     Handles window resize events.
    /// </summary>
    /// <param name="e">Resize event arguments.</param>
    public void ResizeScene(ResizeEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        ResizeSystems.Run();
    }

    /// <summary>
    ///     Called during scene initialization. Override to load resources and configure entities.
    /// </summary>
    protected abstract void OnLoad();
}
