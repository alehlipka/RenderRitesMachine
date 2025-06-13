using OpenTK.Windowing.Common;
using RenderRitesMachine.ECS;

namespace RenderRitesMachine.Output;

public abstract class Scene(string name) : Resource(name)
{
    protected readonly World World = new();
    
    public void UpdateScene(FrameEventArgs args)
    {
        if (!IsLoaded) return;
        
        World.Update((float)args.Time);
    }
    
    public void RenderScene(FrameEventArgs args)
    {
        if (!IsLoaded) return;
        
        World.Render((float)args.Time);
    }
    
    public void ResizeScene(ResizeEventArgs e)
    {
        if (!IsLoaded) return;
        
        World.Resize(e.Width, e.Height);
    }
}