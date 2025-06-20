using Leopotam.EcsLite;
using OpenTK.Windowing.Common;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Output;

public abstract class Scene : IDisposable
{
    public string Name { get; }
    
    protected readonly EcsWorld World;
    protected readonly EcsSystems UpdateSystems;
    protected readonly EcsSystems RenderSystems;
    protected readonly EcsSystems ResizeSystems;
    
    private bool _isLoaded;
    private readonly TimeService _timeService;

    protected Scene(string name)
    {
        _timeService = new TimeService();
        
        Name = name;
        World = new EcsWorld();
        UpdateSystems = new EcsSystems(World, _timeService);
        RenderSystems = new EcsSystems(World, _timeService);
        ResizeSystems = new EcsSystems(World);
    }

    public void Initialize()
    {
        if (_isLoaded) return;
        _isLoaded = true;
        OnLoad();
        UpdateSystems.Init();
    }

    public void UpdateScene(FrameEventArgs args)
    {
        if (!_isLoaded) return;
        _timeService.UpdateDeltaTime = (float)args.Time;
        UpdateSystems.Run();
    }

    public void RenderScene(FrameEventArgs args)
    {
        if (!_isLoaded) return;
        _timeService.RenderDeltaTime = (float)args.Time;
        RenderSystems.Run();
    }

    public void ResizeScene(ResizeEventArgs e)
    {
        if (!_isLoaded) return;
        ResizeSystems.Run();
    }

    public void Dispose()
    {
        if (!_isLoaded) return;
        _isLoaded = false;
        ResizeSystems.Destroy();
        UpdateSystems.Destroy();
        RenderSystems.Destroy();
        World.Destroy();
        GC.SuppressFinalize(this);
    }

    protected abstract void OnLoad();
}