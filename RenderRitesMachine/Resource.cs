namespace RenderRitesMachine;

public abstract class Resource(string name) : IDisposable
{
    public string Name { get; } = name;
    
    internal bool IsLoaded;
    private readonly List<Resource> _dependencies = [];

    public void AddDependency(Resource resource)
    {
        if (_dependencies.Contains(resource)) return;
        
        _dependencies.Add(resource);
    }

    public void Initialize()
    {
        if (IsLoaded) return;
        IsLoaded = true;
        _dependencies.ForEach(d => d.Initialize());
        Load();
    }

    public void Dispose()
    {
        if (!IsLoaded) return;
        IsLoaded = false;
        Unload();
        _dependencies.ForEach(d => d.Dispose());
        GC.SuppressFinalize(this);
    }

    protected abstract void Load();
    protected abstract void Unload();
}