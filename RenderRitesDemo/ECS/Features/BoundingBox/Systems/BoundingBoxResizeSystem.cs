using Leopotam.EcsLite;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Systems;

public class BoundingBoxResizeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        ShaderAsset shader = AssetsService.GetShader("bounding");

        shader.Use();
        shader.SetMatrix4("view", shared.Camera.ViewMatrix);
        shader.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
    }
}
