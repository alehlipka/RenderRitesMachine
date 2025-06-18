using System.Runtime.CompilerServices;
using RenderRitesMachine.ECS.Features.Transform.Components;
using RenderRitesMachine.ECS.Systems;

namespace RenderRitesMachine.ECS.Features.Transform.Systems;

public class TransformUpdateSystem : IUpdateSystem
{
    public void Update(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            transform.Rotation.Rotate(.3f, deltaTime);
        }
    }
}