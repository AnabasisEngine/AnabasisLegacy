using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Abstractions.Buffer;
using Anabasis.Platform.Abstractions.Shaders;
using Anabasis.Platform.Silk.Buffers;
using Anabasis.Utility;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Shader;

public partial class SilkShaderSupport
{
    // FIXME: I think this is fucked for matrices because opengl is column-oriented instead of row-oriented
    private static void GetVertexAttributeBindingCounts(Type type, out int columns, out int countPerColumn) {
        columns = 1;
        switch (type) {
            case not null when type == typeof(Matrix4x4) ||
                               type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Matrix4X4<>):
                columns = 4;
                countPerColumn = 4;
                break;
            case not null when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Matrix3X3<>):
                columns = 3;
                countPerColumn = 3;
                break;
            case not null when type == typeof(Matrix3x2) ||
                               type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Matrix3X2<>):
                columns = 2;
                countPerColumn = 3;
                break;
            case not null when type == typeof(Vector2) ||
                               type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Vector2D<>):
                countPerColumn = 2;
                break;
            case not null when type == typeof(Vector3) ||
                               type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Vector3D<>):
                countPerColumn = 3;
                break;
            case not null when type == typeof(Vector4) ||
                               type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Vector4D<>):
                countPerColumn = 4;
                break;
            default:
                countPerColumn = 1;
                break;
        }
    }

    private static VertexAttribType GetVertexAttribType(Type type) {
        while (true) {
            if (type.IsGenericType) {
                type = type.GetGenericArguments()[0];
                continue;
            }

            if (type == typeof(Half)) return VertexAttribType.HalfFloat;
            return Type.GetTypeCode(type) switch {
                TypeCode.SByte => VertexAttribType.Byte,
                TypeCode.Byte => VertexAttribType.UnsignedByte,
                TypeCode.Int16 => VertexAttribType.Short,
                TypeCode.UInt16 => VertexAttribType.UnsignedShort,
                TypeCode.Int32 => VertexAttribType.Int,
                TypeCode.UInt32 => VertexAttribType.UnsignedInt,
                TypeCode.Single => VertexAttribType.Float,
                TypeCode.Double => VertexAttribType.Double,
                // Unsupported types
                TypeCode.Empty or TypeCode.Object or TypeCode.DBNull or TypeCode.Boolean or TypeCode.Char
                    or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Decimal or TypeCode.DateTime
                    or TypeCode.String => throw new NotSupportedException(),
                // Some other TypeCode that we dont know yet
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }

    public IVertexBufferFormatter<TVertex> CreateVertexFormatter<TVertex>(
        IGraphicsDevice provider,
        IGraphicsHandle programHandle)
        where TVertex : unmanaged {
        GL gl = Guard.IsType<SilkGraphicsDevice>(provider, "Unexpected platform implementation").Gl;
        ProgramHandle handle = Guard.IsType<ProgramHandle>(programHandle);

        Type vertexType = typeof(TVertex);

        IEnumerable<VertexAttribPointer> BuildAttribList() {
            foreach (FieldInfo field in vertexType.GetFields()) {
                if (field.GetCustomAttribute<ShaderUniformAttribute>() is not { } attribute)
                    continue;
                string attributeName = attribute.Name;
                int layout = attribute.Layout ?? gl.GetAttribLocation(handle.Value, attributeName);
                if (layout == -1)
                    continue;
                GetVertexAttributeBindingCounts(field.FieldType, out int cols, out int count);
                for (int i = 0; i < cols; i++) {
                    yield return new VertexAttribPointer((uint)(layout + i), count,
                        GetVertexAttribType(field.FieldType),
                        (uint)Marshal.OffsetOf<TVertex>(field.Name));
                }
            }
        }

        return new LazyBufferBinding<TVertex>(gl, BuildAttribList().ToArray());
    }

    private readonly record struct VertexAttribPointer(uint Layout, [Range(1, 4)] int Count,
        VertexAttribType PointerType, uint Offset);

    private static class LazyBufferBinding
    {
        internal static uint NextBindingIndex;
    }

    private class LazyBufferBinding<TVertex> : IVertexBufferFormatter<TVertex>
        where TVertex : unmanaged
    {
        private          uint?                 _bindingIndex;
        private readonly GL                    _gl;
        private readonly VertexAttribPointer[] _attribs;

        public LazyBufferBinding(GL gl, VertexAttribPointer[] attribs) {
            _gl = gl;
            _attribs = attribs;
        }

        [NotNullIfNotNull(nameof(_bindingIndex))]
        public IBindingIndex? BindingIndex => SilkBindingIndex.FromNullable(_bindingIndex);

        public void BindVertexFormat(IVertexArray<TVertex> array, IBufferObject<TVertex> bufferObject) {
            uint handle = Guard.IsType<ISilkVertexArray<TVertex>>(array).Handle;
            if (_bindingIndex is null) {
                CreateBindings(handle);
            }

            array.BindVertexBuffer(bufferObject, SilkBindingIndex.FromNullable(_bindingIndex));
        }

        [MemberNotNull(nameof(_bindingIndex))]
        // TODO because im like this: this can be optimized using IL emit to create a dynamic implementation type at runtime
        private void CreateBindings(uint handle) {
            _bindingIndex = LazyBufferBinding.NextBindingIndex++;
            // pretty sure roslyn optimizes a foreach on an array to a for loop internally so i think this is fine
            foreach ((uint layout, int count, VertexAttribType pointerType, uint offset) in _attribs) {
                _gl.EnableVertexArrayAttrib(handle, layout);
                _gl.VertexArrayAttribFormat(handle, layout, count, pointerType, false, offset);
                _gl.VertexArrayAttribBinding(handle, layout, (uint)_bindingIndex);
            }
        }
    }
}