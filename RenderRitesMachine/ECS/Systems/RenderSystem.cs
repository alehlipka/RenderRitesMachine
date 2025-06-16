using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.ECS.Components;

namespace RenderRitesMachine.ECS.Systems;

public class RenderSystem : ISystem
{
    public void Update(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            transform.Rotation.Rotate(1.5f, deltaTime);
        }
    }

    public void Render(float deltaTime, World world)
    {
        foreach (ITuple tuple in world.GetComponents(typeof(TransformComponent), typeof(MeshComponent), typeof(ShaderComponent), typeof(TextureComponent)))
        {
            TransformComponent transform = (TransformComponent)tuple[0]!;
            MeshComponent mesh = (MeshComponent)tuple[1]!;
            ShaderComponent shader = (ShaderComponent)tuple[2]!;
            TextureComponent texture = (TextureComponent)tuple[3]!;
            
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
        foreach (ITuple tuple in world.GetComponents(typeof(ShaderComponent), typeof(PerspectiveCameraComponent)))
        {
            ShaderComponent shader = (ShaderComponent)tuple[0]!;
            PerspectiveCameraComponent camera = (PerspectiveCameraComponent)tuple[1]!;
            
            shader.Use();
            shader.SetMatrix4("view", camera.ViewMatrix);
            shader.SetMatrix4("projection", camera.ProjectionMatrix);
        }
    }

    public void Dispose()
    {
        
    }
}