using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;

namespace RenderRitesMachine.ECS.Systems;

public class ResizeSystem() : IResizeSystem
{
    public void Resize(int width, int height, World world)
    {
        GL.Viewport(0, 0, width, height);
        
        foreach (ITuple tuple in world.GetComponents(typeof(ShaderComponent), typeof(PerspectiveCameraComponent)))
        {
            ShaderComponent shader = (ShaderComponent)tuple[0]!;
            PerspectiveCameraComponent camera = (PerspectiveCameraComponent)tuple[1]!;
            
            shader.Use();
            shader.SetMatrix4("view", camera.ViewMatrix);
            shader.SetMatrix4("projection", camera.ProjectionMatrix);
        }
    }
}