using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using GenerateMipmapTarget = OpenTK.Graphics.OpenGL4.GenerateMipmapTarget;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;
using PixelInternalFormat = OpenTK.Graphics.OpenGL4.PixelInternalFormat;
using PixelType = OpenTK.Graphics.OpenGL4.PixelType;
using TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter;
using TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter;
using TextureParameterName = OpenTK.Graphics.OpenGL4.TextureParameterName;
using TextureTarget = OpenTK.Graphics.OpenGL4.TextureTarget;
using TextureWrapMode = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace RenderRitesMachine.GraphicsResources.Textures;

public class Texture(string name, string path) : Resource(name)
{
    private int _handle;
    
    protected override void Load()
    {
        ImageResult? image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
        if (image == null)
        {
            throw new Exception("Texture resource loading error");
        }
        
        _handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _handle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
            image.Width, image.Height, 0, PixelFormat.Rgba, 
            PixelType.UnsignedByte, image.Data);
        
        // Настраиваем фильтрацию
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Настраиваем повторение текстуры
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 8.0f);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, _handle);
    }

    protected override void Unload()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.DeleteTexture(_handle);
    }
}