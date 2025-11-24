using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine;

/// <summary>
/// Главный класс движка RenderRites. Предоставляет точку входа для создания и управления окном рендеринга.
/// </summary>
public sealed class RenderRites : IDisposable
{
    private static readonly Lazy<RenderRites> LazyMachine = new(() => new RenderRites());

    /// <summary>
    /// Единственный экземпляр движка RenderRites (Singleton).
    /// </summary>
    public static RenderRites Machine => LazyMachine.Value;

    /// <summary>
    /// Текущее окно рендеринга. Может быть null до вызова RunWindow.
    /// </summary>
    public Window? Window { get; private set; }

    /// <summary>
    /// Менеджер сцен для управления различными сценами приложения.
    /// </summary>
    public readonly SceneManager Scenes;

    /// <summary>
    /// Общий сервис управления ресурсами для всех сцен.
    /// </summary>
    public readonly IAssetsService AssetsService;

    /// <summary>
    /// Общий сервис времени для всех сцен.
    /// </summary>
    public readonly ITimeService TimeService;

    /// <summary>
    /// Общий сервис рендеринга для всех сцен.
    /// </summary>
    public readonly IRenderService RenderService;

    /// <summary>
    /// Сервис логирования для записи сообщений и ошибок.
    /// </summary>
    public readonly ILogger Logger;

    /// <summary>
    /// Сервис управления аудио для всех сцен.
    /// </summary>
    public readonly IAudioService AudioService;

    /// <summary>
    /// Сервис GUI, предоставляющий поверхность и события.
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
    /// Переопределяет настройки рендеринга для текущего экземпляра движка.
    /// </summary>
    /// <param name="settings">Полный набор пользовательских настроек.</param>
    /// <returns>Текущий экземпляр RenderRites для fluent-конфигурации.</returns>
    public RenderRites ConfigureRenderSettings(RenderSettings settings)
    {
        RenderConstants.Configure(settings);
        return this;
    }

    /// <summary>
    /// Переопределяет настройки рендеринга на основе текущих значений.
    /// </summary>
    /// <param name="configure">Функция, возвращающая изменённую копию настроек.</param>
    /// <returns>Текущий экземпляр RenderRites для fluent-конфигурации.</returns>
    public RenderRites ConfigureRenderSettings(Func<RenderSettings, RenderSettings> configure)
    {
        RenderConstants.Configure(configure);
        return this;
    }

    /// <summary>
    /// Создает и запускает окно рендеринга, используя заданный заголовок или значение по умолчанию.
    /// </summary>
    /// <param name="title">Желаемый заголовок окна или null, чтобы использовать стандартное значение.</param>
    public void RunWindow(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            title = "RenderRites Machine";
        }

        Scenes.Initialize();

        GameWindowSettings gws = new()
        {
            UpdateFrequency = 0.0
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
        Window = new Window(gws, nws, Scenes, GuiService, Logger);
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
