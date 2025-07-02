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

        var shaders = AssetsService.GetAllShaders();
        foreach (ShaderAsset shaderAsset in shaders)
        {
            
            shaderAsset.Use();
            shaderAsset.SetMatrix4("view", shared.Camera.ViewMatrix);
            shaderAsset.SetMatrix4("projection", shared.Camera.ProjectionMatrix);
        }

        Matrix4 view = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, clientSize.X, 0, clientSize.Y, -1, 1);
        ShaderAsset shader = AssetsService.GetShader("text");
        shader.Use();
        shader.SetMatrix4("view", view);
        shader.SetMatrix4("projection", projection);
    }
}