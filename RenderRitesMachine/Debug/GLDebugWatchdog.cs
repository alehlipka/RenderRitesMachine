using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using RenderRitesMachine.Exceptions;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.Debug;

internal static class GlDebugWatchdog
{
    private static ILogger? _logger;

    internal static void Initialize(ILogger? logger = null)
    {
        _logger = logger;
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        DebugProc debugProc = DebugCallback;
        GL.DebugMessageCallback(debugProc, nint.Zero);
        _logger?.LogInfo("OpenGL debug callback initialized");
    }

    private static void DebugCallback(DebugSource source, DebugType type, int i, DebugSeverity severity, int length,
        nint message, nint userParam)
    {
        string messageString = Marshal.PtrToStringAnsi(message, length);

        // Логируем в зависимости от типа и серьезности
        // OpenTK DebugSeverity использует числовые значения: DontCare=0, Low=1, Medium=2, High=3, Notification=0x826B
        LogLevel level = (int)severity switch
        {
            0 => LogLevel.Debug, // DontCare или Notification
            1 => LogLevel.Info, // Low
            2 => LogLevel.Warning, // Medium
            3 => LogLevel.Error, // High
            0x826B => LogLevel.Debug, // Notification
            _ => LogLevel.Info
        };

        if (_logger != null)
        {
            _logger.Log(level, $"[OpenGL] {type} from {source}: {messageString}");
        }
        else
        {
            Console.WriteLine($"[OPENGL DEBUG MESSAGE] {messageString}");
        }
    }

    /// <summary>
    /// Проверяет наличие ошибок OpenGL и выбрасывает исключение, если ошибка обнаружена.
    /// Используется для проверки критических операций OpenGL.
    /// </summary>
    /// <param name="operation">Название операции для сообщения об ошибке.</param>
    /// <exception cref="OpenGLErrorException">Выбрасывается, если обнаружена ошибка OpenGL.</exception>
    internal static void CheckGLError(string operation)
    {
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            _logger?.LogError($"OpenGL error during {operation}: {error}");
            throw new OpenGLErrorException(operation, error);
        }
    }
}
