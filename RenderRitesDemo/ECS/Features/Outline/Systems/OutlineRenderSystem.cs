using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using RenderRitesDemo.ECS.Features.Outline.Components;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Configuration;
using RenderRitesMachine.ECS;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesDemo.ECS.Features.Outline.Systems;

public class OutlineRenderSystem : IEcsRunSystem
{
    // MaxStencilValue moved to RenderConstants

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

        GL.Enable(EnableCap.StencilTest);
        GL.StencilMask(0x00);

        foreach (int entity in filter)
        {
            Mesh mesh = meshes.Get(entity);
            if (!mesh.IsVisible) continue;

            OutlineTag outline = outlines.Get(entity);
            if (!outline.IsVisible) continue;

            Transform transform = transforms.Get(entity);
            MeshAsset meshAsset = shared.Assets.GetMesh(mesh.Name);
            ShaderAsset outlineShaderAsset = shared.Assets.GetShader("outline");

            shared.MarkShaderActive(outlineShaderAsset.Id);

            int stencilId = entity % RenderConstants.MaxStencilValue + 1;
            GL.StencilFunc(StencilFunction.Notequal, stencilId, 0xFF);

            shared.Render.RenderOutline(meshAsset, outlineShaderAsset, transform.ModelMatrix, shared.Camera.Position);
        }

        GL.StencilMask(0xFF);
        GL.Disable(EnableCap.StencilTest);
    }
}
