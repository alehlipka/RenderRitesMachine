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
}