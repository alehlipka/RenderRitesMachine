namespace RenderRitesMachine.Output;

/// <summary>
/// Менеджер для управления сценами приложения. Позволяет добавлять, переключать и управлять сценами.
/// Автоматически добавляет сцену логотипа при создании.
/// </summary>
public class SceneManager : ISceneManager, IDisposable
{
    private readonly Dictionary<string, Scene> _items = new();
    private const string LogoSceneName = "logo";
    private bool _isInitialized;
    private readonly ISceneFactory _sceneFactory;

    /// <summary>
    /// Текущая активная сцена. Может быть null, если сцена не установлена.
    /// </summary>
    public Scene? Current { get; private set; }

    /// <summary>
    /// Создает новый менеджер сцен с указанной фабрикой сцен.
    /// </summary>
    public SceneManager(ISceneFactory sceneFactory)
    {
        _sceneFactory = sceneFactory;
    }

    /// <summary>
    /// Инициализирует менеджер сцен, автоматически добавляя сцену логотипа.
    /// </summary>
    internal void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        // Автоматически добавляем сцену логотипа через фабрику
        var logoScene = _sceneFactory.CreateScene<LogoScene>(LogoSceneName);
        _items.TryAdd(LogoSceneName, logoScene);

        // Автоматически устанавливаем логотип как текущую сцену при первом запуске
        if (Current == null)
        {
            Current = logoScene;
        }
    }

    /// <summary>
    /// Добавляет сцену указанного типа в менеджер, создавая её через фабрику.
    /// </summary>
    /// <typeparam name="T">Тип сцены для создания.</typeparam>
    /// <param name="name">Имя сцены.</param>
    /// <returns>Текущий экземпляр SceneManager для цепочки вызовов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null или пустой.</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается, если пытаются перезаписать встроенную сцену логотипа.</exception>
    public SceneManager AddScene<T>(string name) where T : Scene
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Scene name cannot be null or empty.");
        }

        // Защищаем встроенную сцену логотипа от перезаписи
        if (name == LogoSceneName && _items.ContainsKey(LogoSceneName))
        {
            throw new InvalidOperationException($"Cannot override the built-in '{LogoSceneName}' scene. It is automatically managed by the engine.");
        }

        var scene = _sceneFactory.CreateScene<T>(name);
        _items.TryAdd(name, scene);

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
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector), "Selector cannot be null.");
        }

        return _items.Values.Select(selector);
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
            throw new ArgumentException($"Scene with name '{name}' not found. Available scenes: {string.Join(", ", _items.Keys)}", nameof(name));
        }

        Current = value;
    }

    public void Dispose()
    {
        ForEach(item => item.Dispose());
        GC.SuppressFinalize(this);
    }
}
