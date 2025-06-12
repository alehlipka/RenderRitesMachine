using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace RenderRitesMachine.Utilities;

public static class FpsCounter
{
    // --- CPU FPS ---
    private static int _frameCount;
    private static double _timePassed;
    private static double _lastFps;
    private static readonly Queue<double> FpsHistory = new(60);
    private static double _smoothedFps;
    private static Stopwatch? _cpuStopwatch;

    // --- GPU Time ---
    private static readonly int[] QueryIds = new int[2];
    private static int _currentQueryIndex;
    private static double _lastGpuTimeMs;
    private static readonly Queue<double> GpuTimeHistory = new(60);
    private static double _smoothedGpuTimeMs;
    private static bool _isQueryActive;

    public static double GetFps() => _smoothedFps;
    public static double GetGpuTime() => _smoothedGpuTimeMs;
    public static double GetCpuTime() => _cpuStopwatch?.Elapsed.TotalMilliseconds ?? 0;

    [Conditional("DEBUG")]
    public static void Initialize()
    {
        GL.GenQueries(2, QueryIds);
    }

    [Conditional("DEBUG")]
    public static void BeginGpuMeasure()
    {
        if (_isQueryActive) return;

        GL.QueryCounter(QueryIds[_currentQueryIndex], QueryCounterTarget.Timestamp);
        _isQueryActive = true;
    }

    [Conditional("DEBUG")]
    public static void EndGpuMeasure()
    {
        if (!_isQueryActive)
            return;

        int nextQueryIndex = (_currentQueryIndex + 1) % 2;
        GL.QueryCounter(QueryIds[nextQueryIndex], QueryCounterTarget.Timestamp);
        _isQueryActive = false;

        GL.GetQueryObject(QueryIds[_currentQueryIndex], GetQueryObjectParam.QueryResultAvailable, out int isAvailable);

        if (isAvailable == 1)
        {
            GL.GetQueryObject(QueryIds[_currentQueryIndex], GetQueryObjectParam.QueryResult, out long startTime);
            GL.GetQueryObject(QueryIds[nextQueryIndex], GetQueryObjectParam.QueryResult, out long endTime);

            _lastGpuTimeMs = (endTime - startTime) / 1_000_000.0;

            GpuTimeHistory.Enqueue(_lastGpuTimeMs);
            if (GpuTimeHistory.Count > 60) GpuTimeHistory.Dequeue();
            _smoothedGpuTimeMs = CalculateSmoothed(GpuTimeHistory);
        }

        _currentQueryIndex = nextQueryIndex;
    }

    [Conditional("DEBUG")]
    public static void BeginCpuMeasure()
    {
        _cpuStopwatch = Stopwatch.StartNew();
    }

    [Conditional("DEBUG")]
    public static void EndCpuMeasure()
    {
        _cpuStopwatch?.Restart();
    }

    [Conditional("DEBUG")]
    public static void Update(double deltaTime)
    {
        _frameCount++;
        _timePassed += deltaTime;

        if (!(_timePassed >= 1.0)) return;
        _lastFps = _frameCount / _timePassed;

        FpsHistory.Enqueue(_lastFps);
        if (FpsHistory.Count > 60) FpsHistory.Dequeue();
        _smoothedFps = CalculateSmoothed(FpsHistory);

        _frameCount = 0;
        _timePassed = 0;
    }

    private static double CalculateSmoothed(Queue<double> values)
    {
        if (values.Count == 0) return 0;

        var sorted = new List<double>(values);
        sorted.Sort();

        double median = sorted[sorted.Count / 2];
        double average = sorted.Sum();
        average /= sorted.Count;

        return (median + average) * 0.5;
    }
}