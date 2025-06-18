namespace RenderRitesMachine.ECS.Systems;

public interface IUpdateSystem : ISystem
{
    void Update(float deltaTime, World world);
}