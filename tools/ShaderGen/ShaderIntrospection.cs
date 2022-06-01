using Silk.NET.OpenGL;

namespace ShaderGen;

public static class ShaderIntrospection
{
    public readonly struct UniformInfo
        : IEquatable<UniformInfo>
    {
        public readonly string      Name;
        public readonly UniformType Type;
        public readonly int         ArraySize;

        public UniformInfo(string name, UniformType type, int arraySize) {
            Name = name;
            Type = type;
            ArraySize = arraySize;
        }

        public bool Equals(UniformInfo other) => Name == other.Name && Type == other.Type && ArraySize == other.ArraySize;

        public override bool Equals(object? obj) => obj is UniformInfo other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ ArraySize;
                return hashCode;
            }
        }

        public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Type)}: {Type}, {nameof(ArraySize)}: {ArraySize}";
    }

    public readonly struct VertexAttribInfo
        : IEquatable<VertexAttribInfo>
    {
        public readonly string        Name;
        public readonly int           Count;
        public readonly AttributeType PointerType;
        public readonly int           Location;

        public VertexAttribInfo(string name, int count, AttributeType pointerType, int location) {
            Count = count;
            PointerType = pointerType;
            Location = location;
            Name = name;
        }

        public bool Equals(VertexAttribInfo other) => Name == other.Name && Count == other.Count && PointerType == other.PointerType && Location == other.Location;

        public override bool Equals(object? obj) => obj is VertexAttribInfo other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Count;
                hashCode = (hashCode * 397) ^ (int)PointerType;
                hashCode = (hashCode * 397) ^ Location;
                return hashCode;
            }
        }

        public override string ToString() => $"{nameof(Name)}: {Name}, {nameof(Count)}: {Count}, {nameof(PointerType)}: {PointerType}, {nameof(Location)}: {Location}";
    }

    private static unsafe void GetName(GL gl, uint prog, uint idx, uint len, out string name, ProgramInterface iface) {
        gl.GetProgramResourceName(prog, iface, idx, len, (uint*)0, out name);
    }

    public static IEnumerable<UniformInfo> QueryUniforms(GL gl, uint program) {
        ProgramResourceProperty[] properties = new ProgramResourceProperty[4];
        properties[0] = ProgramResourceProperty.Type;
        properties[1] = ProgramResourceProperty.BlockIndex;
        properties[2] = ProgramResourceProperty.NameLength;
        properties[3] = ProgramResourceProperty.ArraySize;
        gl.GetProgramInterface(program, ProgramInterface.Uniform, ProgramInterfacePName.ActiveResources,
            out int uniformCount);
        int[] values = new int[4];
        for (uint i = 0; i < uniformCount; i++) {
            gl.GetProgramResource(program, ProgramInterface.Uniform, i, 4, properties, 4, Span<uint>.Empty, values);
            if (values[1] != -1)
                continue;
            GetName(gl, program, i, (uint)values[2], out string name, ProgramInterface.Uniform);
            yield return new UniformInfo(name, (UniformType)values[0], values[3]);
        }
    }

    public static IEnumerable<VertexAttribInfo> QueryVertexAttributes(GL gl, uint program) {
        ProgramResourceProperty[] properties = new ProgramResourceProperty[4];
        properties[0] = ProgramResourceProperty.Type;
        properties[1] = ProgramResourceProperty.NameLength;
        properties[2] = ProgramResourceProperty.ArraySize;
        properties[3] = ProgramResourceProperty.Location;
        gl.GetProgramInterface(program, ProgramInterface.ProgramInput, ProgramInterfacePName.ActiveResources,
            out int attribCount);
        int[] values = new int[4];
        for (uint i = 0; i < attribCount; i++) {
            gl.GetProgramResource(program, ProgramInterface.ProgramInput, i, 4, properties, 4, Span<uint>.Empty, values);
            GetName(gl, program, i, (uint)values[1], out string name, ProgramInterface.ProgramInput);
            yield return new VertexAttribInfo(name, values[2], (AttributeType)values[0], values[3]);
        }
    }
}