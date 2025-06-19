using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Assets;

public struct TextureAsset
{
    public int Id;
    
    public void Bind()
    {
        GL.BindTextureUnit(0, Id);
    }
}