using Leopotam.EcsLite;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS.Features.Outline.Systems;

public class OutlineResizeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        ShaderAsset outlineShaderAsset = AssetsService.GetShader("outline");

        outlineShaderAsset.Use();
        outlineShaderAsset.SetMatrix4("view", shared.Camera.ViewMatrix);
        outlineShaderAsset.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
    }
}
