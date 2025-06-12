using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace RenderRitesMachine.ECS.Components;

public readonly struct TextureComponent(string path) : IComponent
{
    public int Handle { get; } = LoadTexture(path);

    private static int LoadTexture(string imagePath)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult? image = ImageResult.FromStream(
            File.OpenRead(imagePath),
            ColorComponents.RedGreenBlueAlpha
        );
        if (image == null)
        {
            throw new Exception("Texture resource loading error");
        }
        
        int handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, handle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
            image.Width, image.Height, 0, PixelFormat.Rgba, 
            PixelType.UnsignedByte, image.Data);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 8.0f);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        return handle;
    }
    
    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.DeleteTexture(Handle);
    }
}