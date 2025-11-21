using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Assets;

namespace RenderRitesMachine.ECS.Systems;

public class MainResizeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        if (shared.Window == null)
        {
            return;
        }

        Vector2i clientSize = shared.Window.ClientSize;
        GL.Viewport(0, 0, clientSize.X, clientSize.Y);
        shared.Camera.AspectRatio = clientSize.X / (float)clientSize.Y;

        foreach (ShaderAsset shaderAsset in shared.Assets.GetAllShaders())
        {
            shaderAsset.Use();
            shaderAsset.SetMatrix4("view", shared.Camera.ViewMatrix);
            shaderAsset.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
        }
    }
}
