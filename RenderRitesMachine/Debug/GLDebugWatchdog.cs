using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Debug;

internal static class GlDebugWatchdog
{
    internal static void Initialize()
    {
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        DebugProc debugProc = DebugCallback;
        GL.DebugMessageCallback(debugProc, nint.Zero);
    }

    private static void DebugCallback(DebugSource source, DebugType type, int i, DebugSeverity severity, int length,
        nint message, nint userParam)
    {
        string messageString = Marshal.PtrToStringAnsi(message, length);
        Console.WriteLine($"[OPENGL DEBUG MESSAGE] {messageString}");
    }

    /// <summary>
    /// Проверяет наличие ошибок OpenGL и выбрасывает исключение, если ошибка обнаружена.
    /// Используется для проверки критических операций OpenGL.
    /// </summary>
    /// <param name="operation">Название операции для сообщения об ошибке.</param>
    /// <exception cref="InvalidOperationException">Выбрасывается, если обнаружена ошибка OpenGL.</exception>
    internal static void CheckGLError(string operation)
    {
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new InvalidOperationException($"OpenGL error during {operation}: {error}");
        }
    }
}
