using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Output;

namespace RenderRitesMachine.Utilities;

public class TextRenderer : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly Shader _shader;
    private Texture2D _fontAtlas;
    private Dictionary<char, FontCharacterInfo> _charInfos;
    
    public TextRenderer(string fontPath, int fontSize = 32, int textureSize = 1024)
    {
        // Генерация SDF атласа
        _fontAtlas = FontSDFGenerator.GenerateSDFFontAtlas(fontPath, fontSize, textureSize, out _charInfos);
        
        // Инициализация шейдера
        _shader = new Shader("Shaders/text.vert", "Shaders/text.frag");
        
        // Настройка VAO/VBO для квадратов символов
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    public void RenderText(string text, Vector2 position, float scale, Vector3 color, 
        float edgeValue = 0.5f, float smoothing = 0.05f)
    {
        _shader.Use();
        _shader.SetVector3("textColor", color);
        _shader.SetFloat("edgeValue", edgeValue);
        _shader.SetFloat("smoothing", smoothing);
        
        GL.ActiveTexture(TextureUnit.Texture0);
        _fontAtlas.Bind();
        
        GL.BindVertexArray(_vao);
        
        float x = position.X;
        float y = position.Y;
        
        foreach (char c in text)
        {
            if (!_charInfos.TryGetValue(c, out var charInfo))
                continue;
                
            float xpos = x + charInfo.XOffset * scale;
            float ypos = y - (charInfo.Height - charInfo.YOffset) * scale;
            
            float w = charInfo.Width * scale;
            float h = charInfo.Height * scale;
            
            // Обновление VBO для текущего символа
            float[] vertices = {
                xpos,     ypos + h, 0.0f, 0.0f,
                xpos,     ypos,     0.0f, 1.0f,
                xpos + w, ypos,     1.0f, 1.0f,
                
                xpos,     ypos + h, 0.0f, 0.0f,
                xpos + w, ypos,     1.0f, 1.0f,
                xpos + w, ypos + h, 1.0f, 0.0f
            };
            
            // Нормализация текстурных координат
            for (int i = 0; i < vertices.Length; i += 4)
            {
                vertices[i+2] = (charInfo.X + vertices[i+2] * charInfo.Width) / _fontAtlas.Width;
                vertices[i+3] = (charInfo.Y + vertices[i+3] * charInfo.Height) / _fontAtlas.Height;
            }
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            
            x += charInfo.XAdvance * scale;
        }
        
        GL.BindVertexArray(0);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }
    
    public void Dispose()
    {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        _fontAtlas.Dispose();
        _shader.Dispose();
    }
}