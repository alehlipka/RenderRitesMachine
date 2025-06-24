using System.Diagnostics;

namespace RenderRitesMachine.Debug;

public static class FpsCounter
{
    private static readonly Stopwatch Stopwatch;
    private static int _frameCount;
    private static double _totalTime;
    private static double _averageFps;

    static FpsCounter()
    {
        Stopwatch = new Stopwatch();
    }
    
    public static void Initialize()
    {
        Stopwatch.Start();
    }

    public static void Update()
    {
        _frameCount++;
        _totalTime = Stopwatch.Elapsed.TotalSeconds;

        if (!(_totalTime >= .5)) return;
        
        _averageFps = _frameCount / _totalTime;
        _frameCount = 0;
        Stopwatch.Restart();
    }
    
    public static double GetFps() => _averageFps;
}