namespace RenderRitesMachine.ECS.Systems.Contracts;

public interface IResizeSystem : ISystem
{
    void Resize(int width, int height, World world);
}