namespace SearchComparisonNet.Kernel.Models;

// Decides when a long-running, indexed loop should emit a progress report. Extracted from the
// view-model so the throttling rule can be unit-tested without a UI or wall-clock timing.
public static class ProgressReportPolicy
{
    // Default minimum gap between progress reports, in milliseconds. Reporting on every iteration
    // floods the UI-thread dispatcher; this bounds reports to roughly five per second.
    public const int DefaultIntervalMs = 200;

    // Report when this is the final iteration, or when at least intervalMs has elapsed since the
    // previous report. Pass lastReportMs = -1 so the first qualifying check always reports.
    public static bool ShouldReport(int index, int count, long elapsedMs, long lastReportMs, int intervalMs = DefaultIntervalMs) =>
        index == count - 1 || elapsedMs - lastReportMs >= intervalMs;
}
