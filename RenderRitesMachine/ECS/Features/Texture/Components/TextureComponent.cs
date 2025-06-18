using OpenTK.Graphics.OpenGL;
using RenderRitesMachine.ECS.Components;
using RenderRitesMachine.Utilities;

namespace RenderRitesMachine.ECS.Features.Texture.Components;

public readonly struct TextureComponent(string path) : IComponent
{
    private readonly int _handle = TextureCreator.Create(path);
    
    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }

    public void Dispose()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.DeleteTexture(_handle);
    }
}