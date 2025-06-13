namespace RenderRitesMachine.ECS.Systems;

public interface ISystem : IDisposable
{
    void Update(float deltaTime, World world);
    
    void Render(float deltaTime, World world);
    
    void Resize(int width, int height, World world);
}