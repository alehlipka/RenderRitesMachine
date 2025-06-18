using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Features.Mesh.Components;
using RenderRitesMachine.ECS.Features.Outline.Components;
using RenderRitesMachine.ECS.Features.Transform.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.Outline.Systems;

public class OutlineRenderSystem : IRenderSystem
{
    public void Render(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent), typeof(MeshComponent), typeof(OutlineShaderComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            MeshComponent mesh = (MeshComponent)tuple[1]!;
            OutlineShaderComponent shader = (OutlineShaderComponent)tuple[2]!;
            
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
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