using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS.Features.Outline.Systems;

public class OutlineRenderSystem : IEcsRunSystem
{
    private readonly Dictionary<string, MeshAsset> _meshAssetCache = new();

    public void Run(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();
        var outlines = world.GetPool<OutlineTag>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<OutlineTag>()
            .End();

        Frustum frustum = shared.GetFrustum();

        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0x00);

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;

            OutlineTag outline = outlines.Get(entity);
            if (!outline.IsVisible) continue;

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

            ShaderAsset outlineShaderAsset = shared.Assets.GetShader("outline");

            shared.MarkShaderActive(outlineShaderAsset.Id);

            int stencilId = entity % RenderConstants.MaxStencilValue + 1;
            GL.StencilFunc(StencilFunction.Notequal, stencilId, 0xFF);

            Vector2i viewportSize = shared.Window?.ClientSize ?? new Vector2i(1920, 1080);
            shared.Render.RenderOutline(meshAsset, outlineShaderAsset, transform.ModelMatrix, shared.Camera.Position, new Vector2(viewportSize.X, viewportSize.Y));
        }

        GL.StencilMask(0xFF);
        GL.Disable(EnableCap.StencilTest);
    }
}
