using Leopotam.EcsLite;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS;

public class IntersectDetectSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        MouseState mouse = RenderRites.Machine.Window!.MouseState;

        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .End();

        foreach (int entity in filter)
        {
            Transform transform = transforms.Get(entity);

            MeshAsset meshAsset = AssetsService.GetMesh(meshes.Get(entity).Name);

            float? hitDistance = Ray
                .GetFromScreen(mouse.X, mouse.Y, shared.Camera.Position, shared.Camera.ProjectionMatrix, shared.Camera.ViewMatrix)
                .TransformToLocalSpace(transform.ModelMatrix)
                .IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum);

            world.GetPool<Outline>().Get(entity).IsVisible = hitDistance != null;
        }
    }
}