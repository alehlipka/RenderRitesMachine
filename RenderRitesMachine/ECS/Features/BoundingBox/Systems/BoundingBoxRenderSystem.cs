using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Features.BoundingBox.Components;
using RenderRitesMachine.ECS.Features.Transform.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.BoundingBox.Systems;

public class BoundingBoxRenderSystem : IRenderSystem
{
    public void Render(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(
            typeof(TransformComponent),
            typeof(BoundingBoxComponent),
            typeof(BoundingBoxShaderComponent)
        ))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            BoundingBoxComponent box = (BoundingBoxComponent)tuple[1]!;
            BoundingBoxShaderComponent shader = (BoundingBoxShaderComponent)tuple[2]!;
            
            shader.Use();
            shader.SetMatrix4("model", transform.ModelMatrix);
            GL.BindVertexArray(box.Vao);
            GL.DrawElements(box.PrimitiveType, box.Count, box.DrawElementsType, box.IndicesStoreLocation);
        }
    }
}