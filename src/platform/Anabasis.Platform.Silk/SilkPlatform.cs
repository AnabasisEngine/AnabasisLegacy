using Anabasis.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SilkNativeWindow = Silk.NET.Windowing.Window;

namespace Anabasis.Platform.Silk;

public sealed class SilkPlatform : IAnabasisPlatform
{
    private IHostApplicationLifetime _lifetime;
    
    private static readonly GraphicsAPI GraphicsApi = new(ContextAPI.OpenGL, ContextProfile.Core,
        ContextFlags.ForwardCompatible,
        new APIVersion(4, 3));

    public SilkPlatform(IHostApplicationLifetime lifetime) {
        _lifetime = lifetime;
    }

    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {
        services.TryAddSingleton<SilkPlatform>();
        services.TryAddSingleton<IAnabasisPlatform>(s => s.GetRequiredService<SilkPlatform>());
        services.TryAddSingleton(sp => (SilkGraphicsDevice)sp.GetRequiredService<IGraphicsDevice>());
    }

    public IAnabasisWindow Window { get; private set; } = null!;
    public IGraphicsDevice GraphicsDevice { get; private set; } = null!;

    public void CreateGraphicsContext() {
        WindowOptions options = new() {
            IsVisible = true,
            Position = new Vector2D<int>(50, 50),
            Size = new Vector2D<int>(1280, 720),
            FramesPerSecond = 0,
            UpdatesPerSecond = 0,
            Title = "Anabasis",
            API = GraphicsApi,
            WindowState = WindowState.Normal,
            WindowBorder = WindowBorder.Resizable,
            VSync = true,
            ShouldSwapAutomatically = true,
            VideoMode = VideoMode.Default,
        };
        var window = SilkNativeWindow.Create(options);
        Window = new SilkWindow(window, _lifetime);
        GraphicsDevice  = new SilkGraphicsDevice(Window);
    }
}