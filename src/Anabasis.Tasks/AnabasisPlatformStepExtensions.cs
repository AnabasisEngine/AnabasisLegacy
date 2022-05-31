namespace Anabasis.Tasks;

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

    public static AnabasisPlatformStepMask ToMask(this AnabasisPlatformLoopStep step) =>
        (AnabasisPlatformStepMask)(1 << (int)step);

    public static AnabasisPlatformStepMask ToMask(this IEnumerable<AnabasisPlatformLoopStep> steps) =>
        steps.Aggregate(AnabasisPlatformStepMask.None, (m, s) => m | s.ToMask());
}