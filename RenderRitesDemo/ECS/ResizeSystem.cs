using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class ResizeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        Vector2i clientSize = RenderRites.Machine.Window!.ClientSize;
        GL.Viewport(0, 0, clientSize.X, clientSize.Y);
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        shared.Camera.AspectRatio = clientSize.X / (float)clientSize.Y;

        Matrix4 viewMatrix = shared.Camera.ViewMatrix;
        Matrix4 projectionMatrix = shared.Camera.ProjectionMatrix;

        ShaderAsset shaderAsset = AssetsService.GetShader("cel");
        shaderAsset.Use();
        shaderAsset.SetMatrix4("view", viewMatrix);
        shaderAsset.SetMatrix4("projection", projectionMatrix);

        ShaderAsset boundingShaderAsset = AssetsService.GetShader("bounding");
        boundingShaderAsset.Use();
        boundingShaderAsset.SetMatrix4("view", viewMatrix);
        boundingShaderAsset.SetMatrix4("projection", projectionMatrix);
    }
}