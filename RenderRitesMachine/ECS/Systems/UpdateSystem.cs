using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems.Contracts;

namespace RenderRitesMachine.ECS.Systems;

public class UpdateSystem : IUpdateSystem
{
    private bool _isWireFrame;
    
    public void Update(float deltaTime, World world)
    {
        if (RenderRites.Machine.Window!.IsKeyPressed(Keys.W))
        {
            _isWireFrame = !_isWireFrame;
            GL.PolygonMode(TriangleFace.FrontAndBack, _isWireFrame ? PolygonMode.Line : PolygonMode.Fill);
        }
        
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            transform.Rotation.Rotate(1.5f, deltaTime);
        }
    }
}