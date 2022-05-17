using System.Collections.ObjectModel;

namespace Anabasis.Platform.Abstractions;

public enum AnabasisPlatformLoopStep : byte
{
    Initialization,
    PostInitialization,
    PreUpdate,
    Update,
    PostUpdate,
    PreRender,
    Render,
    PostRender,
    // TimeUpdate,
}