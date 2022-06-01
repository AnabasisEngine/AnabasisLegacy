using Microsoft.CodeAnalysis;

namespace ShaderGen.Diagnostics;

internal static class DiagnosticDescriptors
{
    private static readonly DiagnosticDescriptor MustBePartialDescriptor = new("ANA002",
        "Target vertex struct must be partial",
        "Cannot generate vertex formatting members for {0} because it is not partial", "VertexFormatGenerator",
        DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor MissingAttributesDescriptor = new("ANA003",
        "Necessary vertex format attribute types not found",
        "Cannot generate vertex formatting members because vertex formatting attribute types do not exist", "VertexFormatGenerator",
        DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor MissingFieldAttributeDescriptor = new("ANA004",
        "Missing vertex attribute info on field",
        "Cannot generate vertex formatting members for {0} because {1} is lacking VertexAttributeAttribute or FieldOffsetAttribute", 
        "VertexFormatGenerator",
        DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor MissingTypeAttributeDescriptor = new("ANA005",
        "Missing vertex attribute info on field",
        "Cannot generate vertex formatting members for {0} because it is lacking StructLayoutAttribute", 
        "VertexFormatGenerator",
        DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor UnexpectedErrorDescriptor = new("ANA001",
        "Unexpected error",
        "An error occurred while generating vertex formatting members for '{0}'\n{1}", "VertexFormatGenerator",
        DiagnosticSeverity.Error, true);

    internal static Diagnostic MissingAttributes(Location location) =>
        Diagnostic.Create(MissingAttributesDescriptor, location);
    internal static Diagnostic MissingFieldAttributes(Location location, string str, string identifier) =>
        Diagnostic.Create(MissingFieldAttributeDescriptor, location, str, identifier);
    internal static Diagnostic MissingTypeAttributes(Location location, string identifier) =>
        Diagnostic.Create(MissingTypeAttributeDescriptor, location, identifier);
    internal static Diagnostic TargetStructIsNotPartial(Location location, string identifier) =>
        Diagnostic.Create(MustBePartialDescriptor, location, identifier);
    
    internal static Diagnostic UnexpectedError(Location location, string identifier, Exception exception) =>
        Diagnostic.Create(UnexpectedErrorDescriptor, location, identifier, exception.Message);
}