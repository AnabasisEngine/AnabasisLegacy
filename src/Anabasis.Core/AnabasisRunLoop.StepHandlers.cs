using Anabasis.Tasks;

namespace Anabasis.Core;

public sealed partial class AnabasisRunLoop
{
    private readonly record struct PlatformLoopHandler(string Name, int Priority, Action Handler)
    {
        public void Invoke() => Handler();
    }

    public readonly struct PlatformLoopHandlerDisposer
        : IDisposable
    {
        internal PlatformLoopHandlerDisposer(AnabasisPlatformLoopStep Step, string Name,
            WeakReference<AnabasisRunLoop> Platform) {
            this.Step = Step;
            this.Name = Name;
            this.Platform = Platform;
        }

        public void Dispose() {
            if (Platform.TryGetTarget(out AnabasisRunLoop? tgt))
                tgt.RemoveHandler(Step, Name);
        }

        internal AnabasisPlatformLoopStep Step { get; init; }
        internal string Name { get; init; }
        internal WeakReference<AnabasisRunLoop> Platform { get; init; }

        internal void Deconstruct(out AnabasisPlatformLoopStep Step, out string Name, out WeakReference<AnabasisRunLoop> Platform) {
            Step = this.Step;
            Name = this.Name;
            Platform = this.Platform;
        }
    }

    public void RemoveHandler(AnabasisPlatformLoopStep step, string name) {
        lock (_loopHandlers) {
            LinkedList<PlatformLoopHandler> list = _loopHandlers[step];
            for (LinkedListNode<PlatformLoopHandler>? node = list.First; node != null; node = node.Next) {
                if (!node.Value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) continue;
                list.Remove(node);
                return;
            }
        }
    }

    private readonly Dictionary<AnabasisPlatformLoopStep, LinkedList<PlatformLoopHandler>> _loopHandlers =
        Enum.GetValues<AnabasisPlatformLoopStep>().ToDictionary(s => s,
            _ => new LinkedList<PlatformLoopHandler>());

    public PlatformLoopHandlerDisposer RegisterHandler(AnabasisPlatformLoopStep step, int priority, string name, Action handler) {
        lock (_loopHandlers) {
            PlatformLoopHandlerDisposer disposer = new(step, name, new WeakReference<AnabasisRunLoop>(this));
            LinkedList<PlatformLoopHandler> list = _loopHandlers[step];

            PlatformLoopHandler loopHandler = new(name, priority, handler);
            for (LinkedListNode<PlatformLoopHandler>? node = list.Last; node != null; node = node.Previous) {
                if (node.Value.Priority > priority) continue;
                list.AddAfter(node, loopHandler);
                return disposer;
            }

            // If we never added, nothing is higher priority than the given so add first.
            list.AddFirst(loopHandler);

            return disposer;
        }
    }

    internal void RunHandlers(AnabasisPlatformLoopStep step) {
        PlatformLoopHandler[] list;
        lock (_loopHandlers) {
            list = _loopHandlers[step].ToArray();
        }

        foreach (PlatformLoopHandler handler in list) {
            handler.Invoke();
        }
    }
}