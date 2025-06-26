using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;

namespace RenderRitesDemo.ECS;

public class MainUpdateSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        RenderRitesMachine.Output.Window window = RenderRites.Machine.Window!;

        if (window.IsKeyPressed(Keys.W))
        {
            PolygonMode currentMode = (PolygonMode)GL.GetInteger(GetPName.PolygonMode);
            GL.PolygonMode(TriangleFace.FrontAndBack,
                currentMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill
            );
        }

        if (window.IsKeyPressed(Keys.F))
        {
            window.WindowState = window.WindowState != WindowState.Fullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal;
        }
    }
}