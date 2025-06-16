namespace RenderRitesMachine.ECS.Systems.Contracts;

public interface IUpdateSystem : ISystem
{
    void Update(float deltaTime, World world);
}