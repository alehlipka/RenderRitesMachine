using System.Runtime.CompilerServices;
using RenderRitesMachine.ECS.Features.CelShader.Components;
using RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.CelShader.Systems;

public class CelShaderResizeSystem : IResizeSystem
{
    public void Resize(int width, int height, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(CelShaderComponent), typeof(PerspectiveCameraComponent)))
        {
            CelShaderComponent shader = (CelShaderComponent)tuple[0]!;
            PerspectiveCameraComponent camera = (PerspectiveCameraComponent)tuple[1]!;
            
            shader.Use();
            shader.SetMatrix4("view", camera.ViewMatrix);
            shader.SetMatrix4("projection", camera.ProjectionMatrix);
        }
    }
}