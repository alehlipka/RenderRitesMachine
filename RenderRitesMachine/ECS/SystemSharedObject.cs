using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RenderRitesMachine.Output;
using RenderRitesMachine.Services;

namespace RenderRitesMachine.ECS;

public class SystemSharedObject(
    PerspectiveCamera camera,
    ITimeService time,
    IAssetsService assets,
    IRenderService render,
    IGuiService gui,
    ISceneManager sceneManager)
{
    public PerspectiveCamera Camera = camera;
    public ITimeService Time = time;
    public IAssetsService Assets = assets;
    public IRenderService Render = render;
    public IGuiService Gui = gui;
    public ISceneManager SceneManager = sceneManager;
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
        // Оптимизированная проверка: используем Vector4 для сравнения строк матрицы
        // Это быстрее, чем проверка каждого элемента отдельно
        return Math.Abs(a.Row0.X - b.Row0.X) < epsilon && Math.Abs(a.Row0.Y - b.Row0.Y) < epsilon &&
               Math.Abs(a.Row0.Z - b.Row0.Z) < epsilon && Math.Abs(a.Row0.W - b.Row0.W) < epsilon &&
               Math.Abs(a.Row1.X - b.Row1.X) < epsilon && Math.Abs(a.Row1.Y - b.Row1.Y) < epsilon &&
               Math.Abs(a.Row1.Z - b.Row1.Z) < epsilon && Math.Abs(a.Row1.W - b.Row1.W) < epsilon &&
               Math.Abs(a.Row2.X - b.Row2.X) < epsilon && Math.Abs(a.Row2.Y - b.Row2.Y) < epsilon &&
               Math.Abs(a.Row2.Z - b.Row2.Z) < epsilon && Math.Abs(a.Row2.W - b.Row2.W) < epsilon &&
               Math.Abs(a.Row3.X - b.Row3.X) < epsilon && Math.Abs(a.Row3.Y - b.Row3.Y) < epsilon &&
               Math.Abs(a.Row3.Z - b.Row3.Z) < epsilon && Math.Abs(a.Row3.W - b.Row3.W) < epsilon;
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
