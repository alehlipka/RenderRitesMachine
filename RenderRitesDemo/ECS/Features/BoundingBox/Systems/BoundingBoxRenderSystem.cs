using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Systems;

public class BoundingBoxRenderSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        EcsPool<Transform>? transforms = world.GetPool<Transform>();
        EcsPool<Mesh>? meshes = world.GetPool<Mesh>();
        EcsPool<BoundingBoxTag>? boundingBoxes = world.GetPool<BoundingBoxTag>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<BoundingBoxTag>()
            .End();

        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        ShaderAsset boundingShaderAsset = shared.Assets.GetShader("bounding");
        shared.MarkShaderActive(boundingShaderAsset.Id);

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;

            BoundingBoxTag box = boundingBoxes.Get(entity);
            if (!box.IsVisible) continue;

            Transform transform = transforms.Get(entity);

            if (!_meshAssetCache.TryGetValue(mesh.Name, out MeshAsset? meshAsset))
            {
                meshAsset = shared.Assets.GetMesh(mesh.Name);
                _meshAssetCache[mesh.Name] = meshAsset;
            }

            BoundingBoxAsset boundingBoxAsset = shared.Assets.GetBoundingBox(mesh.Name);
            shared.Render.Render(boundingBoxAsset.Vao, boundingBoxAsset.IndicesCount, boundingShaderAsset, transform.ModelMatrix, null, PrimitiveType.Lines);
        }
    }
}
