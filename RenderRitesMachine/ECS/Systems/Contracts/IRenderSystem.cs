namespace RenderRitesMachine.ECS.Systems.Contracts;

public interface IRenderSystem : ISystem
{
    void Render(float deltaTime, World world);
}