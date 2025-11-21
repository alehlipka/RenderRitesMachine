using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

/// <summary>
/// Менеджер для управления сценами приложения. Позволяет добавлять, переключать и управлять сценами.
/// </summary>
/// <remarks>
/// Создает новый менеджер сцен с указанной фабрикой сцен.
/// </remarks>
public class SceneManager(ISceneFactory sceneFactory, ILogger? logger = null) : ISceneManager, IDisposable
{
    private readonly Dictionary<string, Scene> _items = [];
    private bool _isInitialized;
    private readonly ISceneFactory _sceneFactory = sceneFactory;
    private readonly ILogger? _logger = logger;

    /// <summary>
    /// Текущая активная сцена. Может быть null, если сцена не установлена.
    /// </summary>
    public Scene? Current { get; private set; }

    /// <summary>
    /// Инициализирует менеджер сцен после добавления пользовательских сцен.
    /// </summary>
    internal void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        _logger?.LogInfo("Initializing SceneManager");

        if (Current == null && _items.Count > 0)
        {
            Current = _items.Values.First();
            _logger?.LogInfo($"Current scene set to '{Current.Name}'");
        }
    }

    /// <summary>
    /// Добавляет сцену указанного типа в менеджер, создавая её через фабрику.
    /// </summary>
    /// <typeparam name="T">Тип сцены для создания.</typeparam>
    /// <param name="name">Имя сцены.</param>
    /// <returns>Текущий экземпляр SceneManager для цепочки вызовов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null или пустой.</exception>
    public SceneManager AddScene<T>(string name) where T : Scene
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Scene name cannot be null or empty.");
        }

        T scene = _sceneFactory.CreateScene<T>(name);
        _ = _items.TryAdd(name, scene);
        _logger?.LogInfo($"Scene '{name}' (type: {typeof(T).Name}) added to SceneManager");

        if (Current == null)
        {
            Current = scene;
            _logger?.LogInfo($"Current scene set to '{name}'");
        }

        return this;
    }

    /// <summary>
    /// Выполняет действие для каждой сцены в менеджере.
    /// </summary>
    /// <param name="action">Действие для выполнения над каждой сценой.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если action равен null.</exception>
    public void ForEach(Action<Scene> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action), "Action cannot be null.");
        }

        foreach (Scene item in _items.Values)
        {
            action(item);
        }
    }

    /// <summary>
    /// Проецирует каждую сцену в результат с помощью указанной функции.
    /// </summary>
    /// <typeparam name="TResult">Тип результата проекции.</typeparam>
    /// <param name="selector">Функция для преобразования сцены в результат.</param>
    /// <returns>Последовательность результатов проекции.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если selector равен null.</exception>
    public IEnumerable<TResult> Select<TResult>(Func<Scene, TResult> selector)
    {
        return selector == null
            ? throw new ArgumentNullException(nameof(selector), "Selector cannot be null.")
            : _items.Values.Select(selector);
    }

    /// <summary>
    /// Переключает текущую активную сцену на указанную по имени во время выполнения приложения.
    /// Начальная сцена устанавливается автоматически при инициализации движка.
    /// </summary>
    /// <param name="name">Имя сцены для переключения.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null или пустой.</exception>
    /// <exception cref="ArgumentException">Выбрасывается, если сцена с указанным именем не найдена.</exception>
    public void SwitchTo(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Scene name cannot be null or empty.");
        }

        if (!_items.TryGetValue(name, out Scene? value))
        {
            _logger?.LogError($"Scene '{name}' not found. Available scenes: {string.Join(", ", _items.Keys)}");
            throw new ArgumentException($"Scene with name '{name}' not found. Available scenes: {string.Join(", ", _items.Keys)}", nameof(name));
        }

        string? previousScene = Current?.Name;
        Current = value;
        _logger?.LogInfo($"Scene switched from '{previousScene ?? "none"}' to '{name}'");
    }

    public void Dispose()
    {
        try
        {
            ForEach(item => item?.Dispose());
        }
        catch (Exception ex)
        {
            _logger?.LogException(LogLevel.Error, ex, "Error disposing SceneManager");
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }
}
