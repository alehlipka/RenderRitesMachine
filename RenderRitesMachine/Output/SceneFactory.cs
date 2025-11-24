using System.Reflection;
using RenderRitesMachine.Services;
using RenderRitesMachine.Services.Gui;

namespace RenderRitesMachine.Output;

/// <summary>
/// Реализация фабрики сцен, которая инжектирует зависимости.
/// </summary>
public class SceneFactory(IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IAudioService audioService, IGuiService guiService, ILogger logger) : ISceneFactory
{
    private readonly IAssetsService _assetsService = assetsService;
    private readonly ITimeService _timeService = timeService;
    private readonly IRenderService _renderService = renderService;
    private readonly IAudioService _audioService = audioService;
    private readonly IGuiService _guiService = guiService;
    private readonly ILogger _logger = logger;
    private ISceneManager? _sceneManager;

    /// <summary>
    /// Устанавливает менеджер сцен. Должен быть вызван после создания SceneManager.
    /// </summary>
    public void SetSceneManager(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;
    }

    public T CreateScene<T>(string name) where T : Scene
    {
        ConstructorInfo? constructor = typeof(T).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            [typeof(string), typeof(IAssetsService), typeof(ITimeService), typeof(IRenderService), typeof(IAudioService), typeof(IGuiService), typeof(ISceneManager), typeof(ILogger)],
            null) ?? throw new InvalidOperationException(
                $"Type {typeof(T).Name} must have a constructor with parameters: (string, IAssetsService, ITimeService, IRenderService, IAudioService, IGuiService, ISceneManager, ILogger)");
        return _sceneManager == null
            ? throw new InvalidOperationException("SceneManager must be set before creating scenes. Call SetSceneManager first.")
            : (T)constructor.Invoke([name, _assetsService, _timeService, _renderService, _audioService, _guiService, _sceneManager, _logger]);
    }
}
