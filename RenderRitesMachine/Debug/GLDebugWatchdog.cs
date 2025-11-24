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

        LogLevel level = (int)severity switch
        {
            0 => LogLevel.Debug,
            1 => LogLevel.Info,
            2 => LogLevel.Warning,
            3 => LogLevel.Error,
            0x826B => LogLevel.Debug,
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
    /// Checks for OpenGL errors and throws when one is detected. Use around critical operations.
    /// </summary>
    /// <param name="operation">Operation name for diagnostics.</param>
    /// <exception cref="OpenGLErrorException">Thrown when an OpenGL error occurs.</exception>
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
