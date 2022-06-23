using Anabasis.Tasks;

namespace Anabasis.Core;

public static class AnabasisTime
{
    public static TimeSpan TotalTime { get; private set; }
    public static TimeSpan TimeSinceLastUpdate { get; private set; }
    public static TimeSpan TimeSinceLastRender { get; private set; }
    
    public static ulong FrameCount { get; private set; }
    
    public static double FramesPerSecond { get; private set; }
    
    public static double UpdatesPerSecond { get; private set; }

    internal static void Update(double dt) {
        AnabasisTask.TotalElapsed = TotalTime += TimeSinceLastUpdate = TimeSpan.FromSeconds(dt);
        UpdatesPerSecond = 1 / dt;
    }

    internal static void Render(double dt) {
        FrameCount++;
        AnabasisTask.Frame = FrameCount;
        TimeSinceLastRender = TimeSpan.FromSeconds(dt);
        FramesPerSecond = 1 / dt;
    }
}