using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Systems;

public class MainRenderSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = [];

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

        shared.RenderStats.Reset();

        List<RenderItem> renderItems = [];

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);

            shared.RenderStats.TotalObjects++;

            Transform transform = transforms.Get(entity);

            if (!_meshAssetCache.TryGetValue(mesh.Name, out MeshAsset? meshAsset))
            {
                meshAsset = shared.Assets.GetMesh(mesh.Name);
                _meshAssetCache[mesh.Name] = meshAsset;
            }

            ColorTexture colorTexture = textures.Get(entity);
            ShaderAsset shaderAsset = shared.Assets.GetShader(mesh.ShaderName);
            TextureAsset textureAsset = shared.Assets.GetTexture(colorTexture.Name);

            renderItems.Add(new RenderItem
            {
                Entity = entity,
                ModelMatrix = transform.ModelMatrix,
                MeshAsset = meshAsset,
                ShaderAsset = shaderAsset,
                TextureAsset = textureAsset
            });
        }

        Vector3 cameraPos = shared.Camera.Position;

        SortByDistance(renderItems, cameraPos);

        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0xFF);

        int currentShaderId = -1;

        foreach (RenderItem item in renderItems)
        {
            if (currentShaderId != item.ShaderAsset.Id)
            {
                shared.MarkShaderActive(item.ShaderAsset.Id);
                currentShaderId = item.ShaderAsset.Id;
            }

            int stencilId = item.Entity % RenderConstants.MaxStencilValue + 1;
            GL.StencilFunc(StencilFunction.Gequal, stencilId, 0xFF);

            shared.Render.Render(item.MeshAsset.Vao, item.MeshAsset.IndicesCount, item.ShaderAsset, item.ModelMatrix,
                item.TextureAsset, PrimitiveType.Triangles);
            shared.RenderStats.RenderedObjects++;
        }

        GL.Disable(EnableCap.StencilTest);
    }

    private static void SortByDistance(List<RenderItem> items, Vector3 cameraPos)
    {
        for (int i = 0; i < items.Count; i++)
        {
            RenderItem item = items[i];
            Vector3 objectPos = item.ModelMatrix.Row3.Xyz;
            item.DistanceSquared = (objectPos - cameraPos).LengthSquared;
            items[i] = item;
        }

        items.Sort((a, b) => a.DistanceSquared.CompareTo(b.DistanceSquared));
    }

    private struct RenderItem
    {
        public int Entity;
        public Matrix4 ModelMatrix;
        public MeshAsset MeshAsset;
        public ShaderAsset ShaderAsset;
        public TextureAsset TextureAsset;
        public float DistanceSquared;
    }
}
