using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.ECS.Systems;

public class MainRenderSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();
    private readonly Dictionary<BatchKey, List<BatchItem>> _batchesCache = new();

    private struct BatchKey : IEquatable<BatchKey>
    {
        public string MeshName;
        public string ShaderName;
        public string TextureName;

        public bool Equals(BatchKey other)
        {
            return MeshName == other.MeshName && ShaderName == other.ShaderName && TextureName == other.TextureName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MeshName, ShaderName, TextureName);
        }
    }

    private struct BatchItem
    {
        public int Entity;
        public Matrix4 ModelMatrix;
        public MeshAsset MeshAsset;
    }

    private static List<BatchItem> SortByDistance(List<BatchItem> items, Vector3 cameraPos)
    {
        return items.OrderBy(item =>
        {
            Vector3 objectPos = item.ModelMatrix.Row3.Xyz;
            return (objectPos - cameraPos).LengthSquared;
        }).ToList();
    }

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        EcsPool<Transform>? transforms = world.GetPool<Transform>();
        EcsPool<Mesh>? meshes = world.GetPool<Mesh>();
        EcsPool<ColorTexture>? textures = world.GetPool<ColorTexture>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<ColorTexture>()
            .End();

        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        Frustum frustum = shared.GetFrustum();

        shared.RenderStats.Reset();

        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0xFF);

        foreach (List<BatchItem> batchList in _batchesCache.Values)
        {
            batchList.Clear();
        }
        _batchesCache.Clear();
        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);

            if (!mesh.IsVisible) continue;

            shared.RenderStats.TotalObjects++;

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
                    shared.RenderStats.CulledObjects++;
                    continue;
                }
            }

            ColorTexture colorTexture = textures.Get(entity);
            BatchKey key = new BatchKey
            {
                MeshName = mesh.Name,
                ShaderName = mesh.ShaderName,
                TextureName = colorTexture.Name
            };

            if (!_batchesCache.TryGetValue(key, out List<BatchItem>? batchList))
            {
                batchList = new List<BatchItem>();
                _batchesCache[key] = batchList;
            }

            batchList.Add(new BatchItem
            {
                Entity = entity,
                ModelMatrix = transform.ModelMatrix,
                MeshAsset = meshAsset
            });
        }

        Vector3 cameraPos = shared.Camera.Position;

        foreach ((BatchKey key, var batchList) in _batchesCache)
        {
            if (batchList == null || batchList.Count == 0) continue;

            MeshAsset meshAsset = batchList[0].MeshAsset;
            if (meshAsset == null) continue;

            ShaderAsset shaderAsset = shared.Assets.GetShader(key.ShaderName);
            TextureAsset textureAsset = shared.Assets.GetTexture(key.TextureName);

            shared.MarkShaderActive(shaderAsset.Id);

            if (batchList.Count > 1 && batchList.Count <= RenderConstants.MaxBatchSize)
            {
                List<BatchItem> sortedItems = SortByDistance(batchList, cameraPos);

                List<Matrix4> modelMatrices = new List<Matrix4>(sortedItems.Count);
                foreach (BatchItem item in sortedItems)
                {
                    modelMatrices.Add(item.ModelMatrix);
                }

                int stencilId = 1;
                GL.StencilFunc(StencilFunction.Gequal, stencilId, 0xFF);

                shared.Render.RenderBatch(meshAsset, shaderAsset, textureAsset, modelMatrices);
                shared.RenderStats.RenderedObjects += batchList.Count;
            }
            else
            {
                List<BatchItem> sortedItems = SortByDistance(batchList, cameraPos);

                foreach (BatchItem item in sortedItems)
                {
                    int stencilId = item.Entity % RenderConstants.MaxStencilValue + 1;
                    GL.StencilFunc(StencilFunction.Gequal, stencilId, 0xFF);

                    shared.Render.Render(meshAsset, shaderAsset, item.ModelMatrix, textureAsset);
                    shared.RenderStats.RenderedObjects++;
                }
            }
        }

        GL.Disable(EnableCap.StencilTest);
    }
}

