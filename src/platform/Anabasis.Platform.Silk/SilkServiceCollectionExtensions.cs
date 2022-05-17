using System.Diagnostics.CodeAnalysis;
using Anabasis.Platform.Silk.Shader.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anabasis.Platform.Silk;

public static class SilkServiceCollectionExtensions
{
    public static void TryAddKnownShaderParameterType
        <TValue, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TParam>(
            this IServiceCollection services)
        where TValue : struct
        where TParam : SilkShaderParameter<TValue> {
        services.TryAddSingleton<ISilkParameterConstructor<TValue>, SilkParameterConstructor<TValue, TParam>>();
    }
}