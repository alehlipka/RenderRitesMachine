namespace RenderRitesMachine.Output;

public class SceneManager : IDisposable
{
    private readonly Dictionary<string, Scene> _items = new();
    public Scene? Current { get; private set; }
    
    public SceneManager Add(Scene item)
    {
        _items.TryAdd(item.Name, item);

        return this;
    }

    public SceneManager AddMany(Scene[] items)
    {
        foreach (Scene item in items)
        {
            Add(item);
        }
        
        return this;
    }
    
    public void ForEach(Action<Scene> action)
    {
        foreach (Scene item in _items.Values)
        {
            action(item);
        }
    }
    
    public IEnumerable<TResult> Select<TResult>(Func<Scene, TResult> selector)
    {
        return _items.Values.Select(selector);
    }

    public void SetCurrent(string name)
    {
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