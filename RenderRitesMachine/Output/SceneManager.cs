namespace RenderRitesMachine.Output;

/// <summary>
/// Менеджер для управления сценами приложения. Позволяет добавлять, переключать и управлять сценами.
/// </summary>
public class SceneManager : IDisposable
{
    private readonly Dictionary<string, Scene> _items = new();
    
    /// <summary>
    /// Текущая активная сцена. Может быть null, если сцена не установлена.
    /// </summary>
    public Scene? Current { get; private set; }
    
    /// <summary>
    /// Добавляет сцену в менеджер.
    /// </summary>
    /// <param name="item">Сцена для добавления.</param>
    /// <returns>Текущий экземпляр SceneManager для цепочки вызовов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если item равен null.</exception>
    public SceneManager Add(Scene item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Scene cannot be null.");
        }

        _items.TryAdd(item.Name, item);

        return this;
    }

    /// <summary>
    /// Добавляет несколько сцен в менеджер.
    /// </summary>
    /// <param name="items">Массив сцен для добавления.</param>
    /// <returns>Текущий экземпляр SceneManager для цепочки вызовов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если items равен null.</exception>
    public SceneManager AddMany(Scene[] items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items), "Scenes array cannot be null.");
        }

        foreach (Scene item in items)
        {
            Add(item);
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
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector), "Selector cannot be null.");
        }

        return _items.Values.Select(selector);
    }

    /// <summary>
    /// Устанавливает текущую активную сцену по имени.
    /// </summary>
    /// <param name="name">Имя сцены для активации.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если name равен null.</exception>
    public void SetCurrent(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Scene name cannot be null or empty.");
        }

        if (!_items.TryGetValue(name, out Scene? value))
        {
            Current = null;
            return;
        }
        
        Current = value;
    }

    public void Dispose()
    {
        ForEach(item => item.Dispose());
        GC.SuppressFinalize(this);
    }
}