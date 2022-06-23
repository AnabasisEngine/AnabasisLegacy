using Anabasis.Core.Graphics.Handles;
using Silk.NET.OpenGL;

namespace Anabasis.Core.Graphics.Shaders;

public abstract class ProgramUniform<T>
{
    protected ProgramUniform(GL gl, string name, ProgramHandle program) {
        Gl = gl;
        Name = name;
        _program = program;
    }

    public string Name { get; }

    public ProgramHandle Program => _program;

    public T Value {
        get => _value;
        set {
            _value = value;
            SetValue(_program.Value, (_location ??= GetLocation())!.Value, value);
        }
    }

    private int? GetLocation() => Gl.GetUniformLocation(_program.Value, Name);

    protected readonly GL        Gl;
    private            T             _value = default!;
    private            int?          _location;
    private readonly   ProgramHandle _program;
    protected abstract void SetValue(uint programHandle, int location, in T value);
}