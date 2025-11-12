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
    // Кэш для MeshAsset по имени меша (избегаем повторных lookup в Dictionary)
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();

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

        Frustum frustum = shared.GetFrustum();
        Vector2i windowSize = shared.Window.ClientSize;
        Ray worldRay = Ray.GetFromScreen(mouse.X, mouse.Y, windowSize, shared.Camera.Position, shared.Camera.ProjectionMatrix, shared.Camera.ViewMatrix);

        // Собираем все объекты, которые пересекаются с лучом, вместе с их расстояниями
        List<(int entity, float distance)> hits = new();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;

            Transform transform = transforms.Get(entity);

            // Кэшируем MeshAsset для избежания повторных lookup
            if (!_meshAssetCache.TryGetValue(mesh.Name, out MeshAsset? meshAsset))
            {
                meshAsset = shared.Assets.GetMesh(mesh.Name);
                _meshAssetCache[mesh.Name] = meshAsset;
            }

            // Frustum culling: проверяем, находится ли объект в пирамиде видимости
            // Если объект не виден, не проверяем попадание мыши
            if (shared.EnableFrustumCulling)
            {
                if (!frustum.IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum, transform.ModelMatrix))
                {
                    continue;
                }
            }

            // Проверяем пересечение луча с AABB в локальном пространстве
            Ray localRay = worldRay.TransformToLocalSpace(transform.ModelMatrix);
            float? localHitDistance = localRay.IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum);

            if (localHitDistance.HasValue)
            {
                // Преобразуем точку пересечения из локального пространства в мировое
                Vector3 localHitPoint = localRay.Origin + localRay.Direction * localHitDistance.Value;
                Vector3 worldHitPoint = Vector3.TransformPosition(localHitPoint, transform.ModelMatrix);

                // Вычисляем расстояние от камеры до точки пересечения
                float worldDistance = (worldHitPoint - shared.Camera.Position).Length;
                hits.Add((entity, worldDistance));
            }
        }

        // Сначала сбрасываем IsVisible для всех объектов
        foreach (int entity in filter)
        {
            world.GetPool<OutlineTag>().Get(entity).IsVisible = false;
        }

        // Устанавливаем IsVisible = true только для ближайшего объекта
        if (hits.Count > 0)
        {
            int closestEntity = hits.MinBy(h => h.distance).entity;
            world.GetPool<OutlineTag>().Get(closestEntity).IsVisible = true;
        }
    }
}
