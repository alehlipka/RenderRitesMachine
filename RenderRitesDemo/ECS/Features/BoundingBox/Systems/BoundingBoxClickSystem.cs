using Leopotam.EcsLite;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Systems;

/// <summary>
/// Система для обработки наведения мыши на объекты с BoundingBox и воспроизведения звука.
/// </summary>
public class BoundingBoxClickSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();
    private int? _clickSoundSourceId;
    private int? _hoveredEntityId = null;

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        if (shared.Window == null) return;

        MouseState mouse = shared.Window.MouseState;

        // Загружаем звук при первом использовании
        if (_clickSoundSourceId == null)
        {
            LoadClickSound(shared);
        }

        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var boundingBoxes = world.GetPool<BoundingBoxTag>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<BoundingBoxTag>()
            .End();

        Frustum frustum = shared.GetFrustum();
        Vector2i windowSize = shared.Window.ClientSize;
        Ray worldRay = Ray.GetFromScreen(mouse.X, mouse.Y, windowSize, shared.Camera.Position, shared.Camera.ProjectionMatrix, shared.Camera.ViewMatrix);

        // Собираем все объекты, которые пересекаются с лучом, вместе с их расстояниями
        List<(int entity, float distance, Vector3 position)> hits = new();

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;

            BoundingBoxTag box = boundingBoxes.Get(entity);
            if (!box.IsVisible) continue;

            Transform transform = transforms.Get(entity);

            // Кэшируем MeshAsset для избежания повторных lookup
            if (!_meshAssetCache.TryGetValue(mesh.Name, out MeshAsset? meshAsset))
            {
                meshAsset = shared.Assets.GetMesh(mesh.Name);
                _meshAssetCache[mesh.Name] = meshAsset;
            }

            // Frustum culling: проверяем, находится ли объект в пирамиде видимости
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
                hits.Add((entity, worldDistance, worldHitPoint));
            }
        }

        // Определяем ближайший объект под курсором
        int? currentHoveredEntity = null;
        Vector3 currentHoveredPosition = Vector3.Zero;
        if (hits.Count > 0)
        {
            var min = hits.MinBy(h => h.distance);
            currentHoveredEntity = min.entity;
            currentHoveredPosition = min.position;
        }

        // Проигрываем звук только при переходе на новый объект (или при первом наведении)
        if (currentHoveredEntity.HasValue && currentHoveredEntity != _hoveredEntityId && _clickSoundSourceId.HasValue)
        {
            shared.Audio.SetSourcePosition(_clickSoundSourceId.Value, currentHoveredPosition);
            shared.Audio.SetListenerPosition(shared.Camera.Position);
            shared.Audio.SetListenerOrientation(shared.Camera.Front, shared.Camera.Up);
            shared.Audio.Play(_clickSoundSourceId.Value);
        }

        // Обновляем ID объекта под курсором
        _hoveredEntityId = currentHoveredEntity;
    }

    private void LoadClickSound(SystemSharedObject shared)
    {
        try
        {
            // Ищем файл click.mp3 в нескольких возможных местах
            string[] audioPaths =
            {
                Path.Combine("Assets", "Sounds", "click.mp3"),
                Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", "click.mp3"),
                Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Sounds", "click.mp3"),
                Path.Combine("..", "..", "..", "..", "RenderRitesDemo", "Assets", "Sounds", "click.mp3")
            };

            string? audioPath = null;
            foreach (string path in audioPaths)
            {
                if (File.Exists(path))
                {
                    audioPath = path;
                    break;
                }
            }

            if (audioPath != null)
            {
                // Загружаем аудио файл
                shared.Audio.LoadAudio("click", audioPath);

                // Создаем источник звука (2D, без позиционирования)
                _clickSoundSourceId = shared.Audio.CreateSource("click", position: null, volume: 1.0f, loop: false);
            }
            else
            {
                shared.Logger.LogWarning("Hover sound file (Assets/Sounds/click.mp3) not found, hover sound will not play");
            }
        }
        catch (Exception ex)
        {
            shared.Logger.LogWarning($"Failed to load hover sound: {ex.Message}");
        }
    }
}

