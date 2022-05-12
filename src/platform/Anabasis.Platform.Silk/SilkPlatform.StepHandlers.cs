using Anabasis.Platform.Abstractions;

namespace Anabasis.Platform.Silk;

public sealed partial class SilkPlatform
{
    private readonly record struct PlatformLoopHandler(string Name, int Priority, Action Handler)
    {
        public void Invoke() => Handler();
    }

    private readonly record struct PlatformLoopHandlerDisposer(AnabasisPlatformLoopStep Step, string Name,
            WeakReference<SilkPlatform> Platform)
        : IDisposable
    {
        public void Dispose() {
            if (Platform.TryGetTarget(out var tgt))
                tgt.RemoveHandler(Step, Name);
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

    public IDisposable RegisterHandler(AnabasisPlatformLoopStep step, int priority, string name, Action handler) {
        lock (_loopHandlers) {
            LinkedList<PlatformLoopHandler> list = _loopHandlers[step];

            PlatformLoopHandlerDisposer disposer = new(step, name, new WeakReference<SilkPlatform>(this));
            PlatformLoopHandler loopHandler = new(name, priority, handler);
            for (LinkedListNode<PlatformLoopHandler>? node = list.Last; node != null; node = node.Previous) {
                if (node.Value.Priority > priority) continue;
                list.AddAfter(node, loopHandler);
                return disposer;
            }

            // If we escape the loop, nothing is higher priority than the given so add first.
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