using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Services.Gui;

/// <summary>
/// Uploads the GUI surface to the GPU and draws it as a fullscreen quad.
/// </summary>
internal sealed class GuiRenderer : IGuiRenderer
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _program;
    private int _textureId;
    private int _textureWidth;
    private int _textureHeight;
    private bool _initialized;

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        float[] vertices =
        {
            // positions   // tex coords (flip Y to match top-left origin surface)
            -1f,  1f,      0f, 0f,
            -1f, -1f,      0f, 1f,
             1f, -1f,      1f, 1f,
             1f,  1f,      1f, 0f
        };

        uint[] indices = { 0, 1, 2, 0, 2, 3 };

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

        _program = CompileShader();

        _textureId = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _textureId);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        _initialized = true;
    }

    public void EnsureTextureSize(int width, int height)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("GuiRenderer must be initialized before resizing.");
        }

        if (width == _textureWidth && height == _textureHeight)
        {
            return;
        }

        _textureWidth = width;
        _textureHeight = height;

        GL.BindTexture(TextureTarget.Texture2D, _textureId);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _textureWidth, _textureHeight, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, nint.Zero);
    }

    public void UploadSurface(GuiSurface surface)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("GuiRenderer is not initialized.");
        }

        EnsureTextureSize(surface.Width, surface.Height);

        if (!surface.IsDirty)
        {
            return;
        }

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        GL.BindTexture(TextureTarget.Texture2D, _textureId);
        ReadOnlySpan<byte> data = surface.Buffer;
        unsafe
        {
            fixed (byte* ptr = data)
            {
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, surface.Width, surface.Height,
                    PixelFormat.Rgba, PixelType.UnsignedByte, (nint)ptr);
            }
        }

        surface.MarkClean();
    }

    public void Render()
    {
        if (!_initialized || _textureWidth == 0 || _textureHeight == 0)
        {
            return;
        }

        bool depthEnabled = GL.IsEnabled(EnableCap.DepthTest);
        GL.Disable(EnableCap.DepthTest);

        GL.BindVertexArray(_vao);
        GL.UseProgram(_program);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _textureId);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        if (depthEnabled)
        {
            GL.Enable(EnableCap.DepthTest);
        }
    }

    public void Dispose()
    {
        if (_program != 0)
        {
            GL.DeleteProgram(_program);
            _program = 0;
        }

        if (_textureId != 0)
        {
            GL.DeleteTexture(_textureId);
            _textureId = 0;
        }

        if (_vao != 0)
        {
            GL.DeleteVertexArray(_vao);
            _vao = 0;
        }

        if (_vbo != 0)
        {
            GL.DeleteBuffer(_vbo);
            _vbo = 0;
        }

        if (_ebo != 0)
        {
            GL.DeleteBuffer(_ebo);
            _ebo = 0;
        }

        _initialized = false;
    }

    private static int CompileShader()
    {
        const string vertexSource = @"#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;
out vec2 TexCoord;
void main()
{
    TexCoord = aTexCoord;
    gl_Position = vec4(aPos.xy, 0.0, 1.0);
}";

        const string fragmentSource = @"#version 330 core
in vec2 TexCoord;
out vec4 FragColor;
uniform sampler2D uTexture;
void main()
{
    FragColor = texture(uTexture, TexCoord);
}";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexStatus);
        if (vertexStatus == 0)
        {
            string log = GL.GetShaderInfoLog(vertexShader);
            throw new InvalidOperationException($"Failed to compile GUI vertex shader: {log}");
        }

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragmentStatus);
        if (fragmentStatus == 0)
        {
            string log = GL.GetShaderInfoLog(fragmentShader);
            throw new InvalidOperationException($"Failed to compile GUI fragment shader: {log}");
        }

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus == 0)
        {
            string log = GL.GetProgramInfoLog(program);
            throw new InvalidOperationException($"Failed to link GUI shader program: {log}");
        }

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        GL.UseProgram(program);
        int uniformLocation = GL.GetUniformLocation(program, "uTexture");
        GL.Uniform1(uniformLocation, 0);
        GL.UseProgram(0);

        return program;
    }
}

