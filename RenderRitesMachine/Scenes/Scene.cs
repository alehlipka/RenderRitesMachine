using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using RenderRitesMachine.Managers;

namespace RenderRitesMachine.Scenes;

public abstract class Scene(string name) : Resource(name)
{
    protected readonly TextureManager TextureManager = new();
    protected readonly ObjectManager ObjectManager = new();
    protected readonly ShaderManager ShaderManager = new();
    
    public void UpdateScene(FrameEventArgs args)
    {
        if (!IsLoaded) return;
        
        Update(args.Time);
    }
    
    public void RenderScene(FrameEventArgs args)
    {
        if (!IsLoaded) return;
        
        Render(args.Time);
    }
    
    public void ResizeScene(ResizeEventArgs e)
    {
        if (!IsLoaded) return;
        
        Resize(e.Width, e.Height);
    }

    protected virtual void Resize(int width, int height)
    {
        if (!IsLoaded) return;
        
        GL.Viewport(0, 0, width, height);
    }
    
    protected abstract void Update(double time);
    protected abstract void Render(double time);
}