using System.Buffers;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Anabasis.Graphics.Abstractions.Internal;

/// <summary>
/// This is just <see cref="PooledDelegates"/> for <see cref="SpanAction{T,TArg}"/> and <see cref="StatelessSpanAction{T}"/>
/// </summary>
internal static class PooledSpanActions
{
    private static class DefaultDelegatePool<T>
        where T : class, new()
    {
        public static readonly ObjectPool<T> Instance = new(() => new T(), 20);
    }

    private static PooledDelegates.Releaser GetPooledDelegate<TPooled, TArg, TUnboundDelegate, TBoundDelegate>(
        TUnboundDelegate unboundDelegate, TArg argument, out TBoundDelegate boundDelegate)
        where TPooled : AbstractDelegateWithBoundArgument<TPooled, TArg, TUnboundDelegate, TBoundDelegate>, new()
        where TUnboundDelegate : Delegate
        where TBoundDelegate : Delegate {
        var obj = DefaultDelegatePool<TPooled>.Instance.Allocate();
        obj.Initialize(unboundDelegate, argument);
        boundDelegate = obj.BoundDelegate;

        return new PooledDelegates.Releaser(obj);
    }

    /// <summary>
    /// Gets an <see cref="Action{T}"/> delegate, which calls <paramref name="unboundAction"/> with the specified
    /// <paramref name="argument"/>. The resulting <paramref name="boundAction"/> may be called any number of times
    /// until the returned <see cref="PooledDelegates.Releaser"/> is disposed.
    /// </summary>
    /// <example>
    /// <para>The following example shows the use of a capturing delegate for a callback action that requires an
    /// argument:</para>
    ///
    /// <code>
    /// int x = 3;
    /// RunWithActionCallback(a => this.DoSomething(a, x));
    /// </code>
    ///
    /// <para>The following example shows the use of a pooled delegate to avoid capturing allocations for the same
    /// callback action:</para>
    ///
    /// <code>
    /// int x = 3;
    /// using var _ = GetPooledAction((a, arg) => arg.self.DoSomething(a, arg.x), (self: this, x), out Action&lt;int&gt; action);
    /// RunWithActionCallback(action);
    /// </code>
    /// </example>
    /// <typeparam name="T1">The type of the first parameter of the bound action.</typeparam>
    /// <typeparam name="TArg">The type of argument to pass to <paramref name="unboundAction"/>.</typeparam>
    /// <param name="unboundAction">The unbound action delegate.</param>
    /// <param name="argument">The argument to pass to the unbound action delegate.</param>
    /// <param name="boundAction">A delegate which calls <paramref name="unboundAction"/> with the specified
    /// <paramref name="argument"/>.</param>
    /// <returns>A disposable <see cref="PooledDelegates.Releaser"/> which returns the object to the delegate pool.</returns>
    public static PooledDelegates.Releaser GetPooledAction<T1, TArg>(SpanAction<T1, TArg> unboundAction, TArg argument,
        out StatelessSpanAction<T1> boundAction)
        => GetPooledDelegate<SpanActionWithBoundArgument<T1, TArg>, TArg, SpanAction<T1, TArg>, StatelessSpanAction<T1>>
            (unboundAction, argument, out boundAction);

    private abstract class AbstractDelegateWithBoundArgument<TSelf, TArg, TUnboundDelegate, TBoundDelegate>
        : PooledDelegates.Poolable
        where TSelf : AbstractDelegateWithBoundArgument<TSelf, TArg, TUnboundDelegate, TBoundDelegate>, new()
        where TUnboundDelegate : Delegate
        where TBoundDelegate : Delegate
    {
        protected AbstractDelegateWithBoundArgument() {
            BoundDelegate = Bind();

            UnboundDelegate = null!;
            Argument = default!;
        }

        public TBoundDelegate BoundDelegate { get; }

        public TUnboundDelegate UnboundDelegate { get; private set; }
        public TArg Argument { get; private set; }

        public void Initialize(TUnboundDelegate unboundDelegate, TArg argument) {
            UnboundDelegate = unboundDelegate;
            Argument = argument;
        }

        public sealed override void ClearAndFree() {
            Argument = default!;
            UnboundDelegate = null!;
            DefaultDelegatePool<TSelf>.Instance.Free((TSelf)this);
        }

        protected abstract TBoundDelegate Bind();
    }

    private sealed class SpanActionWithBoundArgument<T1, TArg>
        : AbstractDelegateWithBoundArgument<SpanActionWithBoundArgument<T1, TArg>, TArg, SpanAction<T1, TArg>,
            StatelessSpanAction<T1>>
    {
        protected override StatelessSpanAction<T1> Bind()
            => arg1 => UnboundDelegate(arg1, Argument);
    }
}