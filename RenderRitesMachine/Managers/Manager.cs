namespace RenderRitesMachine.Managers;

public abstract class Manager<T> : IDisposable where T : Resource
{
    private readonly Dictionary<string, T> _items = new();
    public T? Current { get; private set; }

    public Manager<T> Add(T item)
    {
        _items.TryAdd(item.Name, item);

        return this;
    }

    public Manager<T> AddMany(T[] items)
    {
        foreach (T item in items)
        {
            Add(item);
        }
        
        return this;
    }
    
    public void ForEach(Action<T> action)
    {
        foreach (T item in _items.Values)
        {
            action(item);
        }
    }
    
    public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        return _items.Values.Select(selector);
    }

    public void SetCurrent(string name)
    {
        if (!_items.TryGetValue(name, out T? value))
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