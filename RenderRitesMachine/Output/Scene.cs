using OpenTK.Windowing.Common;
using RenderRitesMachine.ECS;

namespace RenderRitesMachine.Output;

public abstract class Scene(string name) : IDisposable
{
    public string Name { get; } = name;

    protected readonly World World = new();
    
    private bool _isLoaded;
    
    public void Initialize()
    {
        if (_isLoaded) return;
        _isLoaded = true;
        Load();
    }
    
    public void UpdateScene(FrameEventArgs args)
    {
        if (!_isLoaded) return;
        
        World.Update((float)args.Time);
    }
    
    public void RenderScene(FrameEventArgs args)
    {
        if (!_isLoaded) return;
        
        World.Render((float)args.Time);
    }
    
    public void ResizeScene(ResizeEventArgs e)
    {
        if (!_isLoaded) return;
        
        World.Resize(e.Width, e.Height);
    }

    public void Dispose()
    {
        if (!_isLoaded) return;
        _isLoaded = false;
        GC.SuppressFinalize(this);
    }

    protected abstract void Load();
}