using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

namespace ShaderGen;

public static class ShaderGenWindowingContext
{
    public static void CreateWindow(Action<GL> onLoad) {
        WindowOptions options = WindowOptions.Default;
        options.IsVisible = false;
        options.API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 6));
        GlfwWindowing.Use();
        IWindow window = Window.Create(options);
        window.Load += () => {
            onLoad(window.CreateOpenGL());
            window.Close();
        };
        window.Run();
    }
}