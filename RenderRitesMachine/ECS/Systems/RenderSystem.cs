using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems.Contracts;

namespace RenderRitesMachine.ECS.Systems;

public class RenderSystem : IRenderSystem
{
    public void Render(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent), typeof(MeshComponent), typeof(ShaderComponent), typeof(TextureComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            MeshComponent mesh = (MeshComponent)tuple[1]!;
            ShaderComponent shader = (ShaderComponent)tuple[2]!;
            TextureComponent texture = (TextureComponent)tuple[3]!;
            
            texture.Bind();
            shader.Use();
            shader.SetMatrix4("model", transform.ModelMatrix);
            GL.BindVertexArray(mesh.Vao);
            GL.DrawElements(mesh.PrimitiveType, mesh.Count, mesh.DrawElementsType, mesh.IndicesStoreLocation);
        }
        
        #if DEBUG
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent), typeof(BoundingBoxComponent), typeof(ShaderComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            BoundingBoxComponent boundingBox = (BoundingBoxComponent)tuple[1]!;
            ShaderComponent shader = (ShaderComponent)tuple[2]!;
            
            shader.Use();
            shader.SetMatrix4("model", transform.ModelMatrix);
            GL.BindVertexArray(boundingBox.Vao);
            GL.DrawElements(
                boundingBox.PrimitiveType,
                boundingBox.Count,
                boundingBox.DrawElementsType,
                boundingBox.IndicesStoreLocation
            );
        }
        #endif
    }
}