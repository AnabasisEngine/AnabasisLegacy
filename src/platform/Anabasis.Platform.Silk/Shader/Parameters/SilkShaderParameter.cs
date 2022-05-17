using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Internal;

namespace Anabasis.Platform.Silk.Shader.Parameters;

internal abstract class SilkShaderParameter<T> : IShaderParameter<T>
    where T : struct
{
    protected SilkShaderParameter(IGlApi gl, string name, ProgramHandle program) {
        Gl = gl;
        Name = name;
        _program = program;
    }
    public string Name { get; }

    public IPlatformHandle Program => _program;

    public T Value {
        get => _value;
        set {
            _value = value;
            SetValue(_program.Value, (_location ??= GetLocation())!.Value, value);
        }
    }

    private int? GetLocation() => Gl.UniformLocation(_program, Name);

    protected readonly IGlApi        Gl;
    private            T             _value;
    private            int?          _location;
    private readonly   ProgramHandle _program;
    protected abstract void SetValue(uint programHandle, int location, in T value);
}