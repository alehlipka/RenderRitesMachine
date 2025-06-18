namespace RenderRitesMachine.ECS.Systems;

public interface IRenderSystem : ISystem
{
    void Render(float deltaTime, World world);
}