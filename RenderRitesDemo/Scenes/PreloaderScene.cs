using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesDemo.Objects;
using RenderRitesMachine;
using RenderRitesMachine.GraphicsResources.Shader;
using RenderRitesMachine.GraphicsResources.Textures;
using RenderRitesMachine.Output;
using RenderRitesMachine.Scenes;

namespace RenderRitesDemo.Scenes;

public class PreloaderScene(string name) : Scene(name)
{
    private PerspectiveCamera _perspectiveCamera = null!;
    
    protected override void Load()
    {
        _perspectiveCamera = new PerspectiveCamera
        {
            Position = new Vector3(0, 0, 13),
            Target = Vector3.Zero
        };
        
        ShaderManager.Add(
            new ShaderProgram("Default", Path.Combine("Assets", "Shaders"))
        ).SetCurrent("Default");
        
        TextureManager.Add(
            new Texture("debug", Path.Combine("Assets", "Textures", "debug.jpg"))
        ).SetCurrent("debug");
        
        int number = 0;
        for (int x = -9; x <= 9; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                if (x % 2 != 0 && y % 2 == 0)
                {
                    Cube cube = new($"cube{number++}", new Vector3(x, y, 0));
                    cube.Rotation.Angle = 0;
                    cube.Rotation.Axis = new Vector3(
                        RenderRites.Machine.Random.NextSingle(),
                        RenderRites.Machine.Random.NextSingle(),
                        RenderRites.Machine.Random.NextSingle()
                    );
                    ObjectManager.Add(cube);
                }
            }
        }
        
        TextureManager.ForEach(item => item.Initialize());
        ShaderManager.ForEach(item => item.Initialize());
        ObjectManager.ForEach(item => item.Initialize());
        
        RenderRites.Machine.ObjectManager.ForEach(item => item.Initialize());
        RenderRites.Machine.SceneManager.ForEach(item => item.Initialize());
        RenderRites.Machine.ShaderManager.ForEach(item => item.Initialize());
        
        ShaderManager.Current?.Use();
        TextureManager.Current?.Bind();
        ShaderManager.Current?.SetMatrix4("view", _perspectiveCamera.ViewMatrix);
        ShaderManager.Current?.SetMatrix4("projection", _perspectiveCamera.ProjectionMatrix);
    }

    protected override void Unload()
    {
        ShaderManager.Dispose();
        ObjectManager.Dispose();
    }

    protected override void Resize(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
        
        _perspectiveCamera.AspectRatio = width / (float)height;
        ShaderManager.Current?.SetMatrix4("projection", _perspectiveCamera.ProjectionMatrix);
        
        ObjectManager.ForEach(item => item.Resize(width, height));
    }

    protected override void Update(double time)
    {
        ObjectManager.ForEach(item => item.Rotation.Rotate(1, time));
    }

    protected override void Render(double time)
    {
        ObjectManager.ForEach(item =>
        {
            ShaderManager.Current?.SetMatrix4("model", item.ModelMatrix);
            item.Render(time);
        });
    }
}