using System.Runtime.CompilerServices;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.ECS.Systems.Contracts;

namespace RenderRitesMachine.ECS.Systems;

public class BoundingUpdateSystem : IUpdateSystem
{
    public void Update(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(BoundingBoxComponent)))
        {
            BoundingBoxComponent boundingBox = (BoundingBoxComponent)tuple[0]!;
            
            TransformComponent transform = world.GetEntityComponent<TransformComponent>(boundingBox.Parent);
            boundingBox.Transform(transform.ModelMatrix, boundingBox.OriginalMinimum, boundingBox.OriginalMaximum);
        }
    }
}