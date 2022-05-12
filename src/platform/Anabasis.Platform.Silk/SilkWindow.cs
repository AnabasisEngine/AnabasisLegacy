using Anabasis.Platform.Abstractions;
using Silk.NET.Windowing;

namespace Anabasis.Platform.Silk;

public class SilkWindow : IAnabasisWindow
{
    internal IWindow Window { get; }
}