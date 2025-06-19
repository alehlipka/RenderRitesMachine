using Leopotam.EcsLite;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine;

namespace RenderRitesDemo.ECS;

public class UpdateSystem : IEcsRunSystem
{
    private bool _isWireFrame;
    
    public void Run(IEcsSystems systems)
    {
        if (!RenderRites.Machine.Window!.IsKeyPressed(Keys.W)) return;
    
        _isWireFrame = !_isWireFrame;
        GL.PolygonMode(TriangleFace.FrontAndBack, _isWireFrame ? PolygonMode.Line : PolygonMode.Fill);
    }
}