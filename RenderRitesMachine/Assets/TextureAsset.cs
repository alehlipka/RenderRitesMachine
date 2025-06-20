using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

public struct TextureAsset
{
    public int Id;
    public TextureType Type;
    
    public void Bind()
    {
        GL.BindTextureUnit((int)Type, Id);
    }
}