using Leopotam.EcsLite;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Systems;

public class BoundingBoxRenderSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<BoundingBoxTag>()
            .End();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            ShaderAsset boundingShaderAsset = AssetsService.GetShader("bounding");
            BoundingBoxAsset boundingBoxAsset = AssetsService.GetBoundingBox(mesh.Name);
            
            RenderService.Render(boundingBoxAsset, boundingShaderAsset, transform.ModelMatrix);
        }
    }
}
