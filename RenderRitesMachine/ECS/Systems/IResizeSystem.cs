namespace RenderRitesMachine.ECS.Systems;

public interface IResizeSystem : ISystem
{
    void Resize(int width, int height, World world);
}