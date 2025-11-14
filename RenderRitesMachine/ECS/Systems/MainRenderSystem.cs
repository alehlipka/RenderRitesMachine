using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Systems;

public class MainRenderSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();

    private struct RenderItem
    {
        public int Entity;
        public Matrix4 ModelMatrix;
        public MeshAsset MeshAsset;
        public ShaderAsset ShaderAsset;
        public TextureAsset TextureAsset;
    }

    private static List<RenderItem> SortByDistance(List<RenderItem> items, Vector3 cameraPos)
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

        shared.RenderStats.Reset();

        List<RenderItem> renderItems = new();

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
        List<RenderItem> sortedItems = SortByDistance(renderItems, cameraPos);

        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0xFF);

        foreach (RenderItem item in sortedItems)
        {
            shared.MarkShaderActive(item.ShaderAsset.Id);

            int stencilId = item.Entity % RenderConstants.MaxStencilValue + 1;
            GL.StencilFunc(StencilFunction.Gequal, stencilId, 0xFF);

            shared.Render.Render(item.MeshAsset, item.ShaderAsset, item.ModelMatrix, item.TextureAsset);
            shared.RenderStats.RenderedObjects++;
        }

        GL.Disable(EnableCap.StencilTest);
    }
}

