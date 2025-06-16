using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Systems;

public class BoundingBoxRenderSystem : ISystem
{
    public void Update(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(BoundingBoxComponent)))
        {
            BoundingBoxComponent boundingBox = (BoundingBoxComponent)tuple[0]!;
            
            TransformComponent transform = world.GetComponent<TransformComponent>(boundingBox.Parent);
            boundingBox.Transform(transform.ModelMatrix, boundingBox.OriginalMinimum, boundingBox.OriginalMaximum);
        }
    }

    public void Render(float deltaTime, World world)
    {
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
    }

    public void Resize(int width, int height, World world)
    {
        
    }

    public void Dispose()
    {
        
    }
}