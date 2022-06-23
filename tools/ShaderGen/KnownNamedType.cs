using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ShaderGen;

public enum KnownNamedType
{
    Half,
    Color,
    NumericsVector2,
    NumericsVector3,
    SilkVector2,
    SilkVector3,
    SilkVector4,
    VertexTypeAttribute,
    VertexAttributeAttribute,
    FieldOffsetAttribute,
    StructLayoutAttribute,
}

public static class KnownNamedTypes
{
    public static readonly ImmutableDictionary<KnownNamedType, string> TypeMetadataNames =
        ImmutableDictionary<KnownNamedType, string>.Empty
            .Add(KnownNamedType.Half, "System.Half")
            .Add(KnownNamedType.Color, "Anabasis.Core.Color")
            .Add(KnownNamedType.NumericsVector2, "System.Numerics.Vector2")
            .Add(KnownNamedType.NumericsVector3, "System.Numerics.Vector3")
            .Add(KnownNamedType.SilkVector2, "Silk.NET.Maths.Vector2D`1")
            .Add(KnownNamedType.SilkVector3, "Silk.NET.Maths.Vector3D`1")
            .Add(KnownNamedType.SilkVector4, "Silk.NET.Maths.Vector4D`1")
            .Add(KnownNamedType.VertexTypeAttribute, "Anabasis.Core.Graphics.Rendering.VertexTypeAttribute")
            .Add(KnownNamedType.VertexAttributeAttribute, "Anabasis.Core.Graphics.Rendering.VertexAttributeAttribute")
            .Add(KnownNamedType.StructLayoutAttribute, "System.Runtime.InteropServices.StructLayoutAttribute")
            .Add(KnownNamedType.FieldOffsetAttribute, "System.Runtime.InteropServices.FieldOffsetAttribute");

    public static ImmutableDictionary<KnownNamedType, INamedTypeSymbol?> LoadSymbols(Compilation compilation) =>
        TypeMetadataNames.ToImmutableDictionary(pair => pair.Key, pair => compilation
            .GetTypeByMetadataName(pair.Value));
}