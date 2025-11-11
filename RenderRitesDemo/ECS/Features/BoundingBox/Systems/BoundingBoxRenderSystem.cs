using Leopotam.EcsLite;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Systems;

public class BoundingBoxRenderSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var boundingBoxes = world.GetPool<BoundingBoxTag>();
        
        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<BoundingBoxTag>()
            .End();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;
            
            BoundingBoxTag box = boundingBoxes.Get(entity);
            if (!box.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
            ShaderAsset boundingShaderAsset = shared.Assets.GetShader("bounding");
            BoundingBoxAsset boundingBoxAsset = shared.Assets.GetBoundingBox(mesh.Name);
            
            shared.MarkShaderActive(boundingShaderAsset.Id);
            
            RenderService.Render(boundingBoxAsset, boundingShaderAsset, transform.ModelMatrix);
        }
    }
}
