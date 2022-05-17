using System.Diagnostics.CodeAnalysis;
using Anabasis.Graphics.Abstractions.Shaders;
using Anabasis.Platform.Silk.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Anabasis.Platform.Silk.Shader.Parameters;

internal interface ISilkParameterConstructor<TValue>
    where TValue : struct
{
    public IShaderParameter<TValue> Create(IGlApi gl, string name, ProgramHandle programHandle);
}

internal class SilkParameterConstructor<TValue,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TParam>
    : ISilkParameterConstructor<TValue>
    where TValue : struct
    where TParam : SilkShaderParameter<TValue>
{
    private readonly IServiceProvider _provider;
    private readonly ObjectFactory    _factory;

    public SilkParameterConstructor(IServiceProvider provider) {
        _provider = provider;
        _factory = ActivatorUtilities.CreateFactory(typeof(TParam),
            new[] { typeof(IGlApi), typeof(string), typeof(ProgramHandle), });
    }

    public IShaderParameter<TValue> Create(IGlApi gl, string name, ProgramHandle programHandle) {
        return _factory(_provider, new object?[] { gl, name, programHandle, }) as IShaderParameter<TValue> ??
               throw new InvalidOperationException();
    }
}

internal class ParameterConstructorProvider
{
    private readonly IServiceProvider _provider;
    public ParameterConstructorProvider(IServiceProvider provider) {
        _provider = provider;
    }

    public ISilkParameterConstructor<TValue> Get<TValue>()
        where TValue : struct => _provider.GetRequiredService<ISilkParameterConstructor<TValue>>();
}