using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RenderRitesMachine.ECS.Features.Mesh.Components;
using RenderRitesMachine.ECS.Features.Outline.Components;
using RenderRitesMachine.ECS.Features.PerspectiveCamera.Components;
using RenderRitesMachine.ECS.Features.Transform.Components;
using RenderRitesMachine.ECS.Systems;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.ECS.Features.Outline.Systems;

public class OutlineRenderSystem : IRenderSystem
{
    public void Render(float deltaTime, World world)
    {
        MouseState mouse = RenderRites.Machine.Window!.MouseState;
        
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent), typeof(MeshComponent), typeof(OutlineShaderComponent), typeof(PerspectiveCameraComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            MeshComponent mesh = (MeshComponent)tuple[1]!;
            OutlineShaderComponent shader = (OutlineShaderComponent)tuple[2]!;
            PerspectiveCameraComponent camera = (PerspectiveCameraComponent)tuple[3]!;
            
            if (mouse.IsButtonDown(MouseButton.Left))
            {
                float? hitDistance = Ray
                    .GetFromScreen(mouse.X, mouse.Y, camera.Position, camera.ProjectionMatrix, camera.ViewMatrix)
                    .TransformToLocalSpace(transform.ModelMatrix)
                    .IntersectsAABB(mesh.Minimum, mesh.Maximum);

                if (hitDistance != null)
                {
                    GL.Disable(EnableCap.DepthTest);
                    GL.CullFace(TriangleFace.Front);
                    shader.Use();
                    shader.SetMatrix4("model", transform.ModelMatrix);
                    GL.BindVertexArray(mesh.Vao);
                    GL.DrawElements(mesh.PrimitiveType, mesh.Count, mesh.DrawElementsType, mesh.IndicesStoreLocation);
                    GL.CullFace(TriangleFace.Back);
                    GL.Enable(EnableCap.DepthTest);
                }
            }
        }
    }
}