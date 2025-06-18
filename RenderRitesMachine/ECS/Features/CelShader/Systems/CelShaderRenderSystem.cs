using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Features.CelShader.Components;
using RenderRitesMachine.ECS.Features.Mesh.Components;
using RenderRitesMachine.ECS.Features.Texture.Components;
using RenderRitesMachine.ECS.Features.Transform.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.CelShader.Systems;

public class CelShaderRenderSystem : IRenderSystem
{
    public void Render(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent), typeof(MeshComponent), typeof(CelShaderComponent), typeof(TextureComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            MeshComponent mesh = (MeshComponent)tuple[1]!;
            CelShaderComponent shader = (CelShaderComponent)tuple[2]!;
            TextureComponent texture = (TextureComponent)tuple[3]!;
            
            texture.Bind();
            shader.Use();
            shader.SetMatrix4("model", transform.ModelMatrix);
            GL.BindVertexArray(mesh.Vao);
            GL.DrawElements(mesh.PrimitiveType, mesh.Count, mesh.DrawElementsType, mesh.IndicesStoreLocation);
        }
    }
}