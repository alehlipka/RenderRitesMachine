using System.Runtime.CompilerServices;
using RenderRitesMachine.ECS.Features.Outline.Components;
using RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.Outline.Systems;

public class OutlineResizeSystem : IResizeSystem
{
    public void Resize(int width, int height, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(OutlineShaderComponent), typeof(PerspectiveCameraComponent)))
        {
            OutlineShaderComponent shader = (OutlineShaderComponent)tuple[0]!;
            PerspectiveCameraComponent camera = (PerspectiveCameraComponent)tuple[1]!;
            
            shader.Use();
            shader.SetMatrix4("view", camera.ViewMatrix);
            shader.SetMatrix4("projection", camera.ProjectionMatrix);
            shader.SetVector3("cameraPosition", camera.Position);
        }
    }
}