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

[Flags]
public enum AnabasisPlatformStepMask : byte
{
    None               = 0x0,
    Initialization     = 0x1,
    PostInitialization = 0x2,
    PreUpdate          = 0x4,
    Update             = 0x8,
    PostUpdate         = 0x10,
    PreRender          = 0x20,
    Render             = 0x40,
    PostRender         = 0x80,
    All                = 0xFF,
}

public static class AnabasisPlatformStepExtensions
{
    public static IEnumerable<AnabasisPlatformLoopStep> IncludedSteps(this AnabasisPlatformStepMask mask) {
        for (int i = 0; i < 8; i++) {
            AnabasisPlatformStepMask step = (AnabasisPlatformStepMask)(1 << i);
            bool b = MaskMatches(mask, step);
            if (b)
                yield return (AnabasisPlatformLoopStep)i;
        }
    }

    public static bool HasStep(this AnabasisPlatformStepMask mask, AnabasisPlatformLoopStep step) =>
        (mask & (AnabasisPlatformStepMask)(1 << (int)step)) != AnabasisPlatformStepMask.None;

    public static bool MaskMatches(this AnabasisPlatformStepMask mask, AnabasisPlatformStepMask step) =>
        (mask & step) != AnabasisPlatformStepMask.None;

    public static AnabasisPlatformStepMask ToMask(this IEnumerable<AnabasisPlatformLoopStep> steps) =>
        steps.Aggregate(AnabasisPlatformStepMask.None, (m, s) => m | (AnabasisPlatformStepMask)(1 << (int)s));
}