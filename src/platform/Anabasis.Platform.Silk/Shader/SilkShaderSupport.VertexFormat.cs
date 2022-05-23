using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Anabasis.Graphics.Abstractions.Buffer;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Abstractions;
using Anabasis.Platform.Silk.Buffers;
using Anabasis.Platform.Silk.Internal;
using Anabasis.Utility;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Anabasis.Platform.Silk.Shader;

internal partial class SilkShaderSupport
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
        Type origType = type;
        if (type.IsGenericType) {
            type = type.GetGenericArguments()[0];
        }

        if (type == typeof(Half)) return VertexAttribType.HalfFloat;
        if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4))
            return VertexAttribType.Float;
        TypeCode code = Type.GetTypeCode(type);
        return code switch {
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
                or TypeCode.String => throw new NotSupportedException(
                    $"Unsupported typecode {code} for type {origType}"),
            // Some other TypeCode that we dont know yet
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    internal static IEnumerable<VertexAttribPointer> BuildAttribList<TVertex>(IGlApi gl, ProgramHandle handle) {
        Type vertexType = typeof(TVertex);
        foreach (FieldInfo field in vertexType.GetFields()) {
            if (field.GetCustomAttribute<VertexAttributeAttribute>() is not { } attribute)
                continue;
            string attributeName = attribute.Name;
            int layout = attribute.Layout == -1 ? gl.GetAttribLocation(handle, attributeName) : attribute.Layout;
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

    public IVertexBufferFormatter<TVertex> CreateVertexFormatter<TVertex>(IPlatformHandle programHandle)
        where TVertex : unmanaged {
        ProgramHandle handle = Guard.IsType<ProgramHandle>(programHandle);

        if (typeof(TVertex).GetCustomAttribute<VertexTypeAttribute>() is not { } typeAttribute)
            throw new InvalidOperationException();
        return new LazyBufferBinding<TVertex>(_gl, typeAttribute.Divisor, BuildAttribList<TVertex>(_gl, handle).ToArray());
    }

    internal readonly record struct VertexAttribPointer(uint Layout, [Range(1, 4)] int Count,
        VertexAttribType PointerType, uint Offset);

    private static class LazyBufferBinding
    {
        internal static uint NextBindingIndex;
    }

    private class LazyBufferBinding<TVertex> : IVertexBufferFormatter<TVertex>
        where TVertex : unmanaged
    {
        private readonly IGlApi                _gl;
        private readonly uint                   _divisor;
        private readonly VertexAttribPointer[] _attribs;
        private          SilkBindingIndex?     _bindingIndex;

        public LazyBufferBinding(IGlApi gl, uint divisor, VertexAttribPointer[] attribs) {
            _gl = gl;
            _divisor = divisor;
            _attribs = attribs;
        }

        public IBindingIndex? BindingIndex => _bindingIndex;

        [MemberNotNull(nameof(BindingIndex))]
        public IBindingIndex BindVertexFormat(IVertexArray array, IBufferObject<TVertex> bufferObject) {
            VertexArrayHandle handle = Guard.IsType<IGlObject<VertexArrayHandle>>(array).Handle;
            if (BindingIndex is null) {
                CreateBindings(array, handle, bufferObject);
            } else
                array.BindVertexBuffer(bufferObject, BindingIndex);

            return BindingIndex;
        }

        [MemberNotNull(nameof(BindingIndex))]
        private void CreateBindings(IVertexArray array, VertexArrayHandle handle, IBufferObject<TVertex> bufferObject) {
            _bindingIndex = new SilkBindingIndex(LazyBufferBinding.NextBindingIndex++);
            array.BindVertexBuffer(bufferObject, _bindingIndex);
            // pretty sure roslyn optimizes a foreach on an array to a for loop internally so i think this is fine
            foreach ((uint layout, int count, VertexAttribType pointerType, uint offset) in _attribs) {
                _gl.EnableVertexArrayAttrib(handle, layout);
                _gl.VertexArrayAttribFormat(handle, layout, count, pointerType, false, offset);
                _gl.VertexArrayAttribBinding(handle, layout, _bindingIndex.Value);
            }
            _gl.VertexArrayBindingDivisor(handle, _bindingIndex.Value, _divisor);
            _gl.GetAndThrowError();
#pragma warning disable CS8774
        }
#pragma warning restore CS8774
    }
}