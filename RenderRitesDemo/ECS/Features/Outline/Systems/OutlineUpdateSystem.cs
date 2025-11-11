using Leopotam.EcsLite;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS.Features.Outline.Systems;

public class OutlineUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        
        if (shared.Window == null) return;
        
        MouseState mouse = shared.Window.MouseState;

        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .End();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;
            
            Transform transform = transforms.Get(entity);
            MeshAsset meshAsset = shared.Assets.GetMesh(meshes.Get(entity).Name);

            float? hitDistance = Ray
                .GetFromScreen(mouse.X, mouse.Y, shared.Camera.Position, shared.Camera.ProjectionMatrix, shared.Camera.ViewMatrix)
                .TransformToLocalSpace(transform.ModelMatrix)
                .IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum);

            world.GetPool<OutlineTag>().Get(entity).IsVisible = hitDistance != null;
        }
    }
}
