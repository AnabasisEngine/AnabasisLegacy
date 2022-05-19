using System.Numerics;
using Anabasis.Graphics.Abstractions;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Graphics.Abstractions.Textures;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Shader;
using Anabasis.Platform.Silk.Shader.Parameters;
using Anabasis.Platform.Silk.Textures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SilkNativeWindow = Silk.NET.Windowing.Window;

namespace Anabasis.Platform.Silk;

public sealed class SilkPlatform : IGraphicsPlatform
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILoggerFactory           _loggerFactory;

    private static readonly GraphicsAPI GraphicsApi = new(ContextAPI.OpenGL, ContextProfile.Core,
        ContextFlags.ForwardCompatible, new APIVersion(4, 3));

    public SilkPlatform(IHostApplicationLifetime lifetime, ILoggerFactory loggerFactory) {
        _lifetime = lifetime;
        _loggerFactory = loggerFactory;
    }

    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {
        services.TryAddSingleton<SilkPlatform>();
        services.TryAddSingleton<IAnabasisPlatform>(s => s.GetRequiredService<SilkPlatform>());
        services.TryAddSingleton<IGraphicsPlatform>(s => s.GetRequiredService<SilkPlatform>());
        services.TryAddSingleton(sp => (SilkGraphicsDevice)sp.GetRequiredService<IGraphicsDevice>());
        services.TryAddSingleton<ParameterConstructorProvider>();
        services.TryAddSingleton<IShaderSupport>(sp =>
            new SilkShaderSupport(sp.GetRequiredService<ParameterConstructorProvider>()));
        services.TryAddSingleton<ITextureSupport, SilkTextureSupport>();

        services.TryAddKnownShaderParameterType<Matrix4x4, Matrix4Parameter>();
        services.TryAddKnownShaderParameterType<ITextureBinding, TextureParameter>();
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
        IWindow window = SilkNativeWindow.Create(options);
        Window = new SilkWindow(window, _lifetime, this);
        GraphicsDevice = new SilkGraphicsDevice(Window, _loggerFactory);
    }
}