using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Anabasis.Tasks.CompilerServices;

namespace Anabasis.Tasks;

/// <summary>
/// A more efficient alternative to <c>async void</c> in Anabasis contexts.
/// Call <see cref="Forget"/> to dismiss the warning.
/// </summary>
[AsyncMethodBuilder(typeof(AnabasisVoidTaskAsyncMethodBuilder))]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public struct AnabasisVoidTask
{
    public void Forget() { }
}