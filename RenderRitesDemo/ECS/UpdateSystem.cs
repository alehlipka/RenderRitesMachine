using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.ECS;
using RenderRitesMachine.Services;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS;

public class UpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        RenderRitesMachine.Output.Window window = RenderRites.Machine.Window!;
        MouseState mouse = window.MouseState;

        EcsWorld world = systems.GetWorld();
        SystemSharedObject shared = systems.GetShared<SystemSharedObject>();

        var transforms = world.GetPool<Transform>();
        var meshes = world.GetPool<Mesh>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .End();

        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            transform.RotationAngle += 1.0f * shared.Time.UpdateDeltaTime;
            Matrix4 modelMatrix =
                Matrix4.CreateScale(transform.Scale) *
                Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(transform.RotationAxis, transform.RotationAngle)) *
                Matrix4.CreateTranslation(transform.Position);

            Mesh mesh = meshes.Get(entity);
            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);

            float? hitDistance = Ray
                .GetFromScreen(mouse.X, mouse.Y, shared.Camera.Position, shared.Camera.ProjectionMatrix, shared.Camera.ViewMatrix)
                .TransformToLocalSpace(modelMatrix)
                .IntersectsAABB(meshAsset.Minimum, meshAsset.Maximum);

            if (hitDistance != null)
            {
                world.GetPool<OutlineTag>().Get(entity).IsVisible = true;
            }
            else
            {
                world.GetPool<OutlineTag>().Get(entity).IsVisible = false;
            }
        }

        if (window.IsKeyPressed(Keys.W))
        {
            PolygonMode currentMode = (PolygonMode)GL.GetInteger(GetPName.PolygonMode);
            GL.PolygonMode(TriangleFace.FrontAndBack,
                (currentMode == PolygonMode.Fill) ? PolygonMode.Line : PolygonMode.Fill
            );
        }

        if (window.IsKeyPressed(Keys.F))
        {
            window.WindowState = (window.WindowState != WindowState.Fullscreen)
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }
    }
}