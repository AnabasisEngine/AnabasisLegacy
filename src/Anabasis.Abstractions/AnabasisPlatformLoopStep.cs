namespace Anabasis.Abstractions;

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