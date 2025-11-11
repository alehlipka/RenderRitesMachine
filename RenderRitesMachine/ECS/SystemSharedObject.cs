using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.ECS;

public class SystemSharedObject(PerspectiveCamera camera, TimeService time, AssetsService assets)
{
    public PerspectiveCamera Camera = camera;
    public TimeService Time = time;
    public AssetsService Assets = assets;
    public Window? Window { get; set; }

    private readonly HashSet<int> _activeShaders = [];
    private Matrix4 _lastViewMatrix;
    private Matrix4 _lastProjectionMatrix;
    private bool _matricesInitialized;

    public void MarkShaderActive(int shaderId)
    {
        if (_activeShaders.Add(shaderId))
        {
            UpdateShaderMatrices(shaderId);
        }
    }

    public void UpdateActiveShaders()
    {
        Matrix4 currentView = Camera.ViewMatrix;
        Matrix4 currentProjection = Camera.ProjectionMatrix;

        if (!_matricesInitialized || !MatricesEqual(currentView, _lastViewMatrix) || !MatricesEqual(currentProjection, _lastProjectionMatrix))
        {
            _lastViewMatrix = currentView;
            _lastProjectionMatrix = currentProjection;
            _matricesInitialized = true;

            foreach (int shaderId in _activeShaders)
            {
                UpdateShaderMatrices(shaderId);
            }
        }
    }

    public void ClearActiveShaders()
    {
        _activeShaders.Clear();
    }

    private static bool MatricesEqual(Matrix4 a, Matrix4 b)
    {
        const float epsilon = 0.0001f;
        return Math.Abs(a.M11 - b.M11) < epsilon && Math.Abs(a.M12 - b.M12) < epsilon &&
               Math.Abs(a.M13 - b.M13) < epsilon && Math.Abs(a.M14 - b.M14) < epsilon &&
               Math.Abs(a.M21 - b.M21) < epsilon && Math.Abs(a.M22 - b.M22) < epsilon &&
               Math.Abs(a.M23 - b.M23) < epsilon && Math.Abs(a.M24 - b.M24) < epsilon &&
               Math.Abs(a.M31 - b.M31) < epsilon && Math.Abs(a.M32 - b.M32) < epsilon &&
               Math.Abs(a.M33 - b.M33) < epsilon && Math.Abs(a.M34 - b.M34) < epsilon &&
               Math.Abs(a.M41 - b.M41) < epsilon && Math.Abs(a.M42 - b.M42) < epsilon &&
               Math.Abs(a.M43 - b.M43) < epsilon && Math.Abs(a.M44 - b.M44) < epsilon;
    }

    private void UpdateShaderMatrices(int shaderId)
    {
        GL.UseProgram(shaderId);
        int viewLocation = GL.GetUniformLocation(shaderId, "view");
        int projectionLocation = GL.GetUniformLocation(shaderId, "projection");

        if (viewLocation != -1)
        {
            GL.UniformMatrix4(viewLocation, true, ref _lastViewMatrix);
        }

        if (projectionLocation != -1)
        {
            GL.UniformMatrix4(projectionLocation, true, ref _lastProjectionMatrix);
        }
    }
}
