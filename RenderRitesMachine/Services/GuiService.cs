using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;
using Window = RenderRitesMachine.Output.Window;

namespace RenderRitesMachine.Services;

/// <summary>
/// Сервис для управления GUI через ImGui.
/// </summary>
public class GuiService : IDisposable
{
    private bool _isInitialized;
    private IntPtr _context;
    private int _fontTexture;
    private int _shaderProgram;
    private int _vertexArray;
    private int _vertexBuffer;
    private int _indexBuffer;
    private int _uniformLocationProjection;
    private int _uniformLocationFontTexture;
    private Window? _window;

    /// <summary>
    /// Инициализирует GUI сервис.
    /// </summary>
    /// <param name="window">Окно приложения.</param>
    public void Initialize(Window window)
    {
        if (_isInitialized)
        {
            return;
        }

        _window = window;
        _context = ImGui.CreateContext();
        ImGui.SetCurrentContext(_context);
        
        SetupKeyMap();
        SetupStyle();
        CreateDeviceObjects();
        
        _isInitialized = true;
    }

    /// <summary>
    /// Обновляет состояние GUI (обработка ввода).
    /// </summary>
    /// <param name="deltaTime">Время с последнего кадра.</param>
    public void Update(float deltaTime)
    {
        if (!_isInitialized || _window == null)
        {
            return;
        }

        ImGui.SetCurrentContext(_context);
        
        ImGuiIOPtr io = ImGui.GetIO();
        io.DeltaTime = deltaTime;
        
        Vector2i size = _window.ClientSize;
        io.DisplaySize = new System.Numerics.Vector2(size.X, size.Y);
        io.DisplayFramebufferScale = new System.Numerics.Vector2(1.0f, 1.0f);
        
        UpdateInput(io);
        
        ImGui.NewFrame();
    }

    /// <summary>
    /// Рендерит GUI.
    /// </summary>
    public void Render()
    {
        if (!_isInitialized)
        {
            return;
        }

        ImGui.SetCurrentContext(_context);
        ImGui.Render();
        
        RenderDrawData(ImGui.GetDrawData());
    }

    /// <summary>
    /// Получает контекст ImGui для использования в других частях приложения.
    /// </summary>
    /// <returns>Контекст ImGui или IntPtr.Zero, если не инициализирован.</returns>
    public IntPtr GetContext()
    {
        return _isInitialized ? _context : IntPtr.Zero;
    }

    private void SetupKeyMap()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        
        // Настройка маппинга клавиш для новой версии ImGui.NET
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        
        // Добавляем поддержку клавиш через AddKeyEvent
        // Это будет вызываться в UpdateInput
    }

    private void SetupStyle()
    {
        ImGui.StyleColorsDark();
    }

    private void UpdateInput(ImGuiIOPtr io)
    {
        if (_window == null)
        {
            return;
        }

        KeyboardState keyboard = _window.KeyboardState;
        MouseState mouse = _window.MouseState;
        
        // Обновление модификаторов
        io.AddKeyEvent(ImGuiKey.ModCtrl, keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl));
        io.AddKeyEvent(ImGuiKey.ModShift, keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift));
        io.AddKeyEvent(ImGuiKey.ModAlt, keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt));
        io.AddKeyEvent(ImGuiKey.ModSuper, keyboard.IsKeyDown(Keys.LeftSuper) || keyboard.IsKeyDown(Keys.RightSuper));
        
        // Обновление специальных клавиш
        io.AddKeyEvent(ImGuiKey.Tab, keyboard.IsKeyDown(Keys.Tab));
        io.AddKeyEvent(ImGuiKey.LeftArrow, keyboard.IsKeyDown(Keys.Left));
        io.AddKeyEvent(ImGuiKey.RightArrow, keyboard.IsKeyDown(Keys.Right));
        io.AddKeyEvent(ImGuiKey.UpArrow, keyboard.IsKeyDown(Keys.Up));
        io.AddKeyEvent(ImGuiKey.DownArrow, keyboard.IsKeyDown(Keys.Down));
        io.AddKeyEvent(ImGuiKey.PageUp, keyboard.IsKeyDown(Keys.PageUp));
        io.AddKeyEvent(ImGuiKey.PageDown, keyboard.IsKeyDown(Keys.PageDown));
        io.AddKeyEvent(ImGuiKey.Home, keyboard.IsKeyDown(Keys.Home));
        io.AddKeyEvent(ImGuiKey.End, keyboard.IsKeyDown(Keys.End));
        io.AddKeyEvent(ImGuiKey.Insert, keyboard.IsKeyDown(Keys.Insert));
        io.AddKeyEvent(ImGuiKey.Delete, keyboard.IsKeyDown(Keys.Delete));
        io.AddKeyEvent(ImGuiKey.Backspace, keyboard.IsKeyDown(Keys.Backspace));
        io.AddKeyEvent(ImGuiKey.Space, keyboard.IsKeyDown(Keys.Space));
        io.AddKeyEvent(ImGuiKey.Enter, keyboard.IsKeyDown(Keys.Enter));
        io.AddKeyEvent(ImGuiKey.Escape, keyboard.IsKeyDown(Keys.Escape));
        io.AddKeyEvent(ImGuiKey.A, keyboard.IsKeyDown(Keys.A));
        io.AddKeyEvent(ImGuiKey.C, keyboard.IsKeyDown(Keys.C));
        io.AddKeyEvent(ImGuiKey.V, keyboard.IsKeyDown(Keys.V));
        io.AddKeyEvent(ImGuiKey.X, keyboard.IsKeyDown(Keys.X));
        io.AddKeyEvent(ImGuiKey.Y, keyboard.IsKeyDown(Keys.Y));
        io.AddKeyEvent(ImGuiKey.Z, keyboard.IsKeyDown(Keys.Z));
        
        // Обновление мыши
        io.AddMousePosEvent(mouse.X, mouse.Y);
        io.AddMouseButtonEvent(0, mouse.IsButtonDown(MouseButton.Left));
        io.AddMouseButtonEvent(1, mouse.IsButtonDown(MouseButton.Right));
        io.AddMouseButtonEvent(2, mouse.IsButtonDown(MouseButton.Middle));
        
        if (mouse.ScrollDelta.Y != 0)
        {
            io.AddMouseWheelEvent(0, mouse.ScrollDelta.Y);
        }
        
        // Обновление текста через события текстового ввода
        // В OpenTK 4.x нужно использовать TextInput event, но для простоты оставим пустым
        // Можно добавить обработчик TextInput в Window если нужно
    }

    private void CreateDeviceObjects()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        
        // Загрузка шрифта с поддержкой кириллицы
        // Пробуем несколько возможных путей
        string[] possiblePaths = 
        {
            Path.Combine("Assets", "Fonts", "arial.ttf"),
            Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts", "arial.ttf"),
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Fonts", "arial.ttf")
        };
        
        string? fontPath = null;
        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                fontPath = path;
                break;
            }
        }
        
        // Загружаем шрифт с поддержкой кириллицы
        // Используем кастомный диапазон символов для явного указания базовых + кириллических символов
        unsafe
        {
            // Диапазон символов: базовые (0x0020-0x00FF) + кириллица (0x0400-0x052F)
            // Формат: массив ushort, где каждая пара - начало и конец диапазона, заканчивается 0
            ushort[] customRanges = new ushort[]
            {
                0x0020, 0x00FF, // Базовые символы (латиница, цифры, знаки препинания)
                0x0400, 0x052F, // Кириллица (расширенный диапазон, включает все кириллические символы)
                0 // Завершающий ноль
            };
            
            fixed (ushort* rangesPtr = customRanges)
            {
                IntPtr glyphRanges = (IntPtr)rangesPtr;
                
                if (fontPath != null)
                {
                    try
                    {
                        // Загружаем шрифт Arial с размером 16px и кастомным диапазоном
                        // Это заменит стандартный шрифт полностью
                        io.Fonts.AddFontFromFileTTF(fontPath, 16.0f, null, glyphRanges);
                    }
                    catch
                    {
                        // Если загрузка не удалась, используем стандартный шрифт с кириллицей
                        io.Fonts.AddFontDefault(glyphRanges);
                    }
                }
                else
                {
                    // Если файл не найден, используем стандартный шрифт с кириллицей
                    io.Fonts.AddFontDefault(glyphRanges);
                }
            }
        }
        
        // Создание шрифтовой текстуры
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);
        
        _fontTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, 
            PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        
        io.Fonts.SetTexID((IntPtr)_fontTexture);
        
        // Создание шейдера
        string vertexShaderSource = @"
#version 330 core
layout (location = 0) in vec2 Position;
layout (location = 1) in vec2 UV;
layout (location = 2) in vec4 Color;

uniform mat4 ProjMtx;

out vec2 Frag_UV;
out vec4 Frag_Color;

void main()
{
    Frag_UV = UV;
    Frag_Color = Color;
    gl_Position = ProjMtx * vec4(Position.xy, 0, 1);
}";

        string fragmentShaderSource = @"
#version 330 core
in vec2 Frag_UV;
in vec4 Frag_Color;

uniform sampler2D Texture;

layout (location = 0) out vec4 Out_Color;

void main()
{
    Out_Color = Frag_Color * texture(Texture, Frag_UV);
}";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        
        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);
        
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
        _uniformLocationProjection = GL.GetUniformLocation(_shaderProgram, "ProjMtx");
        _uniformLocationFontTexture = GL.GetUniformLocation(_shaderProgram, "Texture");
        
        // Создание буферов
        _vertexArray = GL.GenVertexArray();
        _vertexBuffer = GL.GenBuffer();
        _indexBuffer = GL.GenBuffer();
        
        GL.BindVertexArray(_vertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 20, 0);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 20, 8);
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, 20, 16);
        
        GL.BindVertexArray(0);
    }

    private unsafe void RenderDrawData(ImDrawDataPtr drawData)
    {
        if (drawData.CmdListsCount == 0)
        {
            return;
        }

        // Сохранение состояния OpenGL
        int lastProgram = GL.GetInteger(GetPName.CurrentProgram);
        int lastTexture = GL.GetInteger(GetPName.TextureBinding2D);
        int lastArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
        int lastElementArrayBuffer = GL.GetInteger(GetPName.ElementArrayBufferBinding);
        int lastVertexArray = GL.GetInteger(GetPName.VertexArrayBinding);
        int lastBlendSrc = GL.GetInteger(GetPName.BlendSrc);
        int lastBlendDst = GL.GetInteger(GetPName.BlendDst);
        int lastBlendEquationRgb = GL.GetInteger(GetPName.BlendEquationRgb);
        int lastBlendEquationAlpha = GL.GetInteger(GetPName.BlendEquationAlpha);
        bool lastEnableBlend = GL.IsEnabled(EnableCap.Blend);
        bool lastEnableCullFace = GL.IsEnabled(EnableCap.CullFace);
        bool lastEnableDepthTest = GL.IsEnabled(EnableCap.DepthTest);
        bool lastEnableScissorTest = GL.IsEnabled(EnableCap.ScissorTest);
        
        // Настройка состояния для рендеринга ImGui
        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.ScissorTest);
        
        // Настройка проекции
        Vector2i displaySize = _window!.ClientSize;
        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(
            0.0f, displaySize.X, displaySize.Y, 0.0f, -1.0f, 1.0f);
        
        GL.UseProgram(_shaderProgram);
        float[] matrixArray = new float[16];
        matrixArray[0] = projection.M11; matrixArray[1] = projection.M12; matrixArray[2] = projection.M13; matrixArray[3] = projection.M14;
        matrixArray[4] = projection.M21; matrixArray[5] = projection.M22; matrixArray[6] = projection.M23; matrixArray[7] = projection.M24;
        matrixArray[8] = projection.M31; matrixArray[9] = projection.M32; matrixArray[10] = projection.M33; matrixArray[11] = projection.M34;
        matrixArray[12] = projection.M41; matrixArray[13] = projection.M42; matrixArray[14] = projection.M43; matrixArray[15] = projection.M44;
        fixed (float* matrixPtr = matrixArray)
        {
            GL.UniformMatrix4(_uniformLocationProjection, 1, false, matrixPtr);
        }
        GL.Uniform1(_uniformLocationFontTexture, 0);
        
        GL.BindVertexArray(_vertexArray);
        
        drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);
        
        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdLists[n];
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, cmdList.VtxBuffer.Size * 20, cmdList.VtxBuffer.Data, 
                BufferUsageHint.DynamicDraw);
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, cmdList.IdxBuffer.Size * 2, cmdList.IdxBuffer.Data, 
                BufferUsageHint.DynamicDraw);
            
            int vtxOffset = 0;
            for (int cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
            {
                ImDrawCmdPtr cmd = cmdList.CmdBuffer[cmdI];
                
                if (cmd.UserCallback != IntPtr.Zero)
                {
                    // Обработка пользовательского колбэка (если нужно)
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, (int)cmd.TextureId);
                    GL.Scissor((int)cmd.ClipRect.X, (int)(displaySize.Y - cmd.ClipRect.W), 
                        (int)(cmd.ClipRect.Z - cmd.ClipRect.X), (int)(cmd.ClipRect.W - cmd.ClipRect.Y));
                    
                    GL.DrawElements(PrimitiveType.Triangles, (int)cmd.ElemCount, 
                        DrawElementsType.UnsignedShort, (IntPtr)(vtxOffset * 2));
                }
                
                vtxOffset += (int)cmd.ElemCount;
            }
        }
        
        // Восстановление состояния OpenGL
        GL.UseProgram(lastProgram);
        GL.BindTexture(TextureTarget.Texture2D, lastTexture);
        GL.BindVertexArray(lastVertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, lastArrayBuffer);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, lastElementArrayBuffer);
        GL.BlendEquationSeparate((BlendEquationMode)lastBlendEquationRgb, (BlendEquationMode)lastBlendEquationAlpha);
        GL.BlendFunc((BlendingFactor)lastBlendSrc, (BlendingFactor)lastBlendDst);
        
        if (lastEnableBlend)
        {
            GL.Enable(EnableCap.Blend);
        }
        else
        {
            GL.Disable(EnableCap.Blend);
        }
        
        if (lastEnableCullFace)
        {
            GL.Enable(EnableCap.CullFace);
        }
        else
        {
            GL.Disable(EnableCap.CullFace);
        }
        
        if (lastEnableDepthTest)
        {
            GL.Enable(EnableCap.DepthTest);
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }
        
        if (lastEnableScissorTest)
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        else
        {
            GL.Disable(EnableCap.ScissorTest);
        }
    }

    public void Dispose()
    {
        if (!_isInitialized)
        {
            return;
        }

        if (_fontTexture != 0)
        {
            GL.DeleteTexture(_fontTexture);
            _fontTexture = 0;
        }

        if (_shaderProgram != 0)
        {
            GL.DeleteProgram(_shaderProgram);
            _shaderProgram = 0;
        }

        if (_vertexBuffer != 0)
        {
            GL.DeleteBuffer(_vertexBuffer);
            _vertexBuffer = 0;
        }

        if (_indexBuffer != 0)
        {
            GL.DeleteBuffer(_indexBuffer);
            _indexBuffer = 0;
        }

        if (_vertexArray != 0)
        {
            GL.DeleteVertexArray(_vertexArray);
            _vertexArray = 0;
        }

        if (_context != IntPtr.Zero)
        {
            ImGui.DestroyContext(_context);
            _context = IntPtr.Zero;
        }

        _isInitialized = false;
    }
}
