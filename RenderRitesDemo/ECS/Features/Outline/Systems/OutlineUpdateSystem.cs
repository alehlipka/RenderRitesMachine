using Leopotam.EcsLite;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS.Features.Outline.Systems;

public class OutlineUpdateSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        if (shared.Window == null) return;

        MouseState mouse = shared.Window.MouseState;

        EcsPool<Transform>? transforms = world.GetPool<Transform>();
        EcsPool<Mesh>? meshes = world.GetPool<Mesh>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .End();

        Vector2i windowSize = shared.Window.ClientSize;
        Ray worldRay = Ray.GetFromScreen(mouse.X, mouse.Y, windowSize, shared.Camera.Position, shared.Camera.ProjectionMatrix, shared.Camera.ViewMatrix);

        List<(int entity, float distance)> hits = new();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;

            Transform transform = transforms.Get(entity);

            if (!_meshAssetCache.TryGetValue(mesh.Name, out MeshAsset? meshAsset))
            {
                meshAsset = shared.Assets.GetMesh(mesh.Name);
                _meshAssetCache[mesh.Name] = meshAsset;
            }

            Ray localRay = worldRay.TransformToLocalSpace(transform.ModelMatrix);
            float? localHitDistance = localRay.IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum);

            if (localHitDistance.HasValue)
            {
                Vector3 localHitPoint = localRay.Origin + localRay.Direction * localHitDistance.Value;
                Vector3 worldHitPoint = Vector3.TransformPosition(localHitPoint, transform.ModelMatrix);

                float worldDistance = (worldHitPoint - shared.Camera.Position).Length;
                hits.Add((entity, worldDistance));
            }
        }

        foreach (int entity in filter)
        {
            world.GetPool<OutlineTag>().Get(entity).IsVisible = false;
        }

        if (hits.Count > 0)
        {
            int closestEntity = hits.MinBy(h => h.distance).entity;
            world.GetPool<OutlineTag>().Get(closestEntity).IsVisible = true;
        }
    }
}
