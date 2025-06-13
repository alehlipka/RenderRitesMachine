using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Systems;

public class RenderSystem : ISystem
{
    public void Update(float deltaTime, World world)
    {
        var updateItems = world.GetComponents<TransformComponent>();
        foreach (TransformComponent transform in updateItems)
        {
            transform.Rotation.Rotate(1.5f, deltaTime);
        }
    }

    public void Render(float deltaTime, World world)
    {
        var renderItems = world.GetComponents<
            TransformComponent,
            MeshComponent,
            ShaderComponent,
            TextureComponent
        >();
        foreach ((
            TransformComponent transform,
            MeshComponent mesh,
            ShaderComponent shader,
            TextureComponent texture
        ) in renderItems)
        {
            texture.Bind();
            shader.Use();
            shader.SetMatrix4("model", transform.ModelMatrix);
            GL.BindVertexArray(mesh.Vao);
            GL.DrawElements(mesh.PrimitiveType, mesh.Count, mesh.DrawElementsType, mesh.IndicesStoreLocation);
        }
    }

    public void Resize(int width, int height, World world)
    {
        GL.Viewport(0, 0, width, height);
        var resizeItems = world.GetComponents<ShaderComponent, PerspectiveCameraComponent>();
        foreach ((ShaderComponent shader, PerspectiveCameraComponent camera) in resizeItems)
        {
            shader.Use();
            shader.SetMatrix4("view", camera.ViewMatrix);
            shader.SetMatrix4("projection", camera.ProjectionMatrix);
        }
    }

    public void Dispose()
    {
        
    }
}