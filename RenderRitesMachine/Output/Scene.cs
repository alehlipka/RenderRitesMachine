using Leopotam.EcsLite;
using OpenTK.Windowing.Common;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

/// <summary>
/// Базовый класс для всех сцен в приложении. Предоставляет ECS мир, системы обновления и рендеринга,
/// камеру и сервис управления ресурсами.
/// </summary>
public abstract class Scene : IDisposable
{
    /// <summary>
    /// Имя сцены.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// ECS мир для управления сущностями и компонентами.
    /// </summary>
    protected readonly EcsWorld World;

    /// <summary>
    /// Системы обновления, выполняющиеся каждый кадр.
    /// </summary>
    protected readonly EcsSystems UpdateSystems;

    /// <summary>
    /// Системы рендеринга, выполняющиеся каждый кадр.
    /// </summary>
    protected readonly EcsSystems RenderSystems;

    /// <summary>
    /// Системы обработки изменения размера окна.
    /// </summary>
    protected readonly EcsSystems ResizeSystems;

    /// <summary>
    /// Камера сцены для управления видом и проекцией.
    /// </summary>
    protected readonly ICamera Camera;

    /// <summary>
    /// Сервис управления ресурсами (меши, шейдеры, текстуры).
    /// </summary>
    protected readonly IAssetsService Assets;

    /// <summary>
    /// Сервис управления аудио.
    /// </summary>
    protected readonly IAudioService Audio;

    private bool _isLoaded;
    private readonly ITimeService _timeService;
    private readonly SystemSharedObject _shared;

    protected Scene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IAudioService audioService, ISceneManager sceneManager, ILogger logger)
        : this(name, assetsService, timeService, renderService, audioService, sceneManager, logger, new PerspectiveCamera())
    {
    }

    protected Scene(string name, IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IAudioService audioService, ISceneManager sceneManager, ILogger logger, ICamera camera)
    {
        ArgumentNullException.ThrowIfNull(camera);

        _timeService = timeService;
        Camera = camera;
        Assets = assetsService;
        Audio = audioService;
        _shared = new SystemSharedObject(Camera, _timeService, Assets, renderService, audioService, sceneManager, logger);

        Name = name;
        World = new EcsWorld();
        UpdateSystems = new EcsSystems(World, _shared);
        RenderSystems = new EcsSystems(World, _shared);
        ResizeSystems = new EcsSystems(World, _shared);
    }

    /// <summary>
    /// Устанавливает окно для доступа из систем через SystemSharedObject.
    /// </summary>
    /// <param name="window">Окно приложения.</param>
    public void SetWindow(Window window)
    {
        _shared.Window = window;
    }

    /// <summary>
    /// Инициализирует сцену. Вызывается автоматически при первом использовании.
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
    /// Обновляет сцену. Вызывается каждый кадр перед рендерингом.
    /// </summary>
    /// <param name="args">Аргументы кадра с информацией о времени.</param>
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
    /// Рендерит сцену. Вызывается каждый кадр.
    /// </summary>
    /// <param name="args">Аргументы кадра с информацией о времени.</param>
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
    /// Обрабатывает изменение размера окна.
    /// </summary>
    /// <param name="e">Аргументы события изменения размера.</param>
    public void ResizeScene(ResizeEventArgs e)
    {
        if (!_isLoaded)
        {
            return;
        }

        ResizeSystems.Run();
    }

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
    /// Вызывается при инициализации сцены. Переопределите этот метод для загрузки ресурсов
    /// и настройки сцены.
    /// </summary>
    protected abstract void OnLoad();
}
