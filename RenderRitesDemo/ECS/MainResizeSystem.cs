using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;

namespace RenderRitesDemo.ECS;

public class MainResizeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        Vector2i clientSize = RenderRites.Machine.Window!.ClientSize;
        GL.Viewport(0, 0, clientSize.X, clientSize.Y);
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();
        shared.Camera.AspectRatio = clientSize.X / (float)clientSize.Y;

        ShaderAsset shaderAsset = AssetsService.GetShader("cel");
        shaderAsset.Use();
        shaderAsset.SetMatrix4("view", shared.Camera.ViewMatrix);
        shaderAsset.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
    }
}