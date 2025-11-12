using Leopotam.EcsLite;
using OpenTK.Mathematics;
using RenderRitesDemo.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS.Features.BoundingBox.Systems;

public class BoundingBoxRenderSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();

    private struct BatchItem
    {
        public Matrix4 ModelMatrix;
        public BoundingBoxAsset BoundingBoxAsset;
    }

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
        Frustum frustum = shared.GetFrustum();

        Dictionary<string, List<BatchItem>> batches = new();

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

            if (shared.EnableFrustumCulling)
            {
                if (!frustum.IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum, transform.ModelMatrix))
                {
                    continue;
                }
            }

            BoundingBoxAsset boundingBoxAsset = shared.Assets.GetBoundingBox(mesh.Name);

            if (!batches.TryGetValue(mesh.Name, out List<BatchItem>? batchList))
            {
                batchList = new List<BatchItem>();
                batches[mesh.Name] = batchList;
            }

            batchList.Add(new BatchItem
            {
                ModelMatrix = transform.ModelMatrix,
                BoundingBoxAsset = boundingBoxAsset
            });
        }

        ShaderAsset boundingShaderAsset = shared.Assets.GetShader("bounding");
        shared.MarkShaderActive(boundingShaderAsset.Id);

        foreach ((string meshName, List<BatchItem> batchList) in batches)
        {
            BoundingBoxAsset boundingBoxAsset = batchList[0].BoundingBoxAsset;

            if (batchList.Count > 1 && batchList.Count <= RenderConstants.MaxBatchSize)
            {
                List<Matrix4> modelMatrices = new List<Matrix4>(batchList.Count);
                foreach (BatchItem item in batchList)
                {
                    modelMatrices.Add(item.ModelMatrix);
                }

                shared.Render.RenderBatch(boundingBoxAsset, boundingShaderAsset, modelMatrices);
            }
            else
            {
                foreach (BatchItem item in batchList)
                {
                    shared.Render.Render(boundingBoxAsset, boundingShaderAsset, item.ModelMatrix);
                }
            }
        }
    }
}
