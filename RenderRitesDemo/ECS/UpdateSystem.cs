using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;
using RenderRitesMachine.Assets;
using RenderRitesMachine.Services;
using RenderRitesMachine.Utilities;

namespace RenderRitesDemo.ECS;

public class UpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        MouseState mouse = RenderRites.Machine.Window!.MouseState;

        EcsWorld world = systems.GetWorld();

        TimeService time = systems.GetShared<TimeService>();

        var transforms = world.GetPool<Transform>();
        var cameras = world.GetPool<PerspectiveCamera>();
        var meshes = world.GetPool<Mesh>();

        EcsFilter filter = world
            .Filter<Transform>()
            .Inc<Mesh>()
            .Inc<PerspectiveCamera>()
            .End();

        foreach (int entity in filter)
        {
            ref Transform transform = ref transforms.Get(entity);
            transform.RotationAngle += 1.0f * time.UpdateDeltaTime;
            Matrix4 modelMatrix =
                Matrix4.CreateScale(transform.Scale) *
                Matrix4.CreateFromQuaternion(Quaternion.FromAxisAngle(transform.RotationAxis, transform.RotationAngle)) *
                Matrix4.CreateTranslation(transform.Position);

            PerspectiveCamera camera = cameras.Get(entity);
            Mesh mesh = meshes.Get(entity);
            MeshAsset meshAsset = AssetsService.GetMesh(mesh.Name);

            float? hitDistance = Ray
                .GetFromScreen(mouse.X, mouse.Y, camera.Position, camera.ProjectionMatrix, camera.ViewMatrix)
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

        if (RenderRites.Machine.Window!.IsKeyPressed(Keys.W))
        {
            PolygonMode currentMode = (PolygonMode)GL.GetInteger(GetPName.PolygonMode);
            GL.PolygonMode(TriangleFace.FrontAndBack, (currentMode == PolygonMode.Fill) ? PolygonMode.Line : PolygonMode.Fill);
        }

        if (RenderRites.Machine.Window!.IsKeyPressed(Keys.F))
        {
            if (RenderRites.Machine.Window!.WindowState != OpenTK.Windowing.Common.WindowState.Fullscreen)
            {
                RenderRites.Machine.Window!.WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen;
            }
            else
            {
                RenderRites.Machine.Window!.WindowState = OpenTK.Windowing.Common.WindowState.Normal;
            }
        }
    }
}