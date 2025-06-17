using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems.Contracts;

namespace RenderRitesMachine.ECS.Systems;

public class ResizeSystem(ShaderComponent outline) : IResizeSystem
{
    private ShaderComponent _outline = outline;

    public void Resize(int width, int height, World world)
    {
        GL.Viewport(0, 0, width, height);
        foreach (ITuple tuple in world.GetComponents(typeof(ShaderComponent), typeof(PerspectiveCameraComponent)))
        {
            ShaderComponent shader = (ShaderComponent)tuple[0]!;
            PerspectiveCameraComponent camera = (PerspectiveCameraComponent)tuple[1]!;
            
            _outline.Use();
            _outline.SetMatrix4("view", camera.ViewMatrix);
            _outline.SetMatrix4("projection", camera.ProjectionMatrix);
            
            shader.Use();
            shader.SetMatrix4("view", camera.ViewMatrix);
            shader.SetMatrix4("projection", camera.ProjectionMatrix);
        }
    }
}