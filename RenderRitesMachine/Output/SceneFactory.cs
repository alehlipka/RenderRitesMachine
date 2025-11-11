using System.Reflection;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

/// <summary>
/// Реализация фабрики сцен, которая инжектирует зависимости.
/// </summary>
public class SceneFactory : ISceneFactory
{
    private readonly IAssetsService _assetsService;
    private readonly ITimeService _timeService;
    private readonly IRenderService _renderService;
    private readonly IGuiService _guiService;
    private ISceneManager? _sceneManager;

    public SceneFactory(IAssetsService assetsService, ITimeService timeService, IRenderService renderService, IGuiService guiService)
    {
        _assetsService = assetsService;
        _timeService = timeService;
        _renderService = renderService;
        _guiService = guiService;
    }

    /// <summary>
    /// Устанавливает менеджер сцен. Должен быть вызван после создания SceneManager.
    /// </summary>
    public void SetSceneManager(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;
    }

    public T CreateScene<T>(string name) where T : Scene
    {
        // Используем рефлексию для создания сцены с правильными параметрами
        // BindingFlags.NonPublic нужен для internal конструкторов (например, LogoScene)
        var constructor = typeof(T).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(string), typeof(IAssetsService), typeof(ITimeService), typeof(IRenderService), typeof(IGuiService), typeof(ISceneManager) },
            null);

        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Type {typeof(T).Name} must have a constructor with parameters: (string, IAssetsService, ITimeService, IRenderService, IGuiService, ISceneManager)");
        }

        if (_sceneManager == null)
        {
            throw new InvalidOperationException("SceneManager must be set before creating scenes. Call SetSceneManager first.");
        }

        return (T)constructor.Invoke(new object[] { name, _assetsService, _timeService, _renderService, _guiService, _sceneManager });
    }
}

