using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Systems;

public class BoundingBoxRenderSystem : ISystem
{
    public void Update(float deltaTime, World world)
    {
        var updateItems = world.GetComponents<BoundingBoxComponent>();
        foreach (BoundingBoxComponent boundingBox in updateItems)
        {
            TransformComponent transform = world.GetComponent<TransformComponent>(boundingBox.Parent);
            boundingBox.Transform(transform.ModelMatrix, boundingBox.OriginalMinimum, boundingBox.OriginalMaximum);
        }
    }

    public void Render(float deltaTime, World world)
    {
        var renderItems =
            world.GetComponents<TransformComponent, BoundingBoxComponent, ShaderComponent>();
        foreach ((
            TransformComponent transform,
            BoundingBoxComponent boundingBox,
            ShaderComponent shader
        ) in renderItems)
        {
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