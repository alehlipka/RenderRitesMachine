using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

/// <summary>
/// Manages application scenes: add, iterate, and switch between them.
/// </summary>
public class SceneManager : ISceneManager, IDisposable
{
    private readonly Dictionary<string, Scene> _items = [];
    private bool _isInitialized;
    private readonly ISceneFactory _sceneFactory;
    private readonly ILogger? _logger;

    /// <summary>
    /// Creates a scene manager with the provided factory and optional logger.
    /// </summary>
    /// <param name="sceneFactory">Factory used to instantiate scenes.</param>
    /// <param name="logger">Logger for diagnostics, or null to disable logging.</param>
    public SceneManager(ISceneFactory sceneFactory, ILogger? logger = null)
    {
        _sceneFactory = sceneFactory ?? throw new ArgumentNullException(nameof(sceneFactory));
        _logger = logger;
    }

    /// <summary>
    /// Currently active scene. May be null if none has been selected yet.
    /// </summary>
    public Scene? Current { get; private set; }

    /// <summary>
    /// Initializes the manager after scenes have been registered.
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
    /// Adds a scene of the given type by creating it through the factory.
    /// </summary>
    /// <typeparam name="T">Scene type to create.</typeparam>
    /// <param name="name">Scene name.</param>
    /// <returns>The current <see cref="SceneManager"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
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
    /// Executes an action for every registered scene.
    /// </summary>
    /// <param name="action">Action to execute for each scene.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is null.</exception>
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
    /// Projects each scene into a result using the provided selector.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="selector">Function converting a scene into a result.</param>
    /// <returns>An enumerable of projection results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="selector"/> is null.</exception>
    public IEnumerable<TResult> Select<TResult>(Func<Scene, TResult> selector)
    {
        return selector == null
            ? throw new ArgumentNullException(nameof(selector), "Selector cannot be null.")
            : _items.Values.Select(selector);
    }

    /// <summary>
    /// Switches to the scene with the specified name at runtime. The initial scene is set automatically.
    /// </summary>
    /// <param name="name">Name of the scene to activate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when the scene cannot be found.</exception>
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
