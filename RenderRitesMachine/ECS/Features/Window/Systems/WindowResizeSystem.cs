using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.Window.Systems;

public class WindowResizeSystem : IResizeSystem
{
    public void Resize(int width, int height, World world)
    {
        GL.Viewport(0, 0, width, height);
    }
}