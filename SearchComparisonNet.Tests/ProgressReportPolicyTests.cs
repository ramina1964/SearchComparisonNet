namespace SearchComparisonNet.Tests;

// Unit tests for the pure throttle predicate extracted from MainViewModel. No UI or wall-clock
// dependency: elapsed time is passed in directly, so the behavior is fully deterministic.
public sealed class ProgressReportPolicyTests
{
    public static TheoryData<int, int, long, long, int, bool> Cases => new()
    {
        // index, count, elapsedMs, lastReportMs, intervalMs, expected
        { 99, 100, 0, 0, 200, true },      // final iteration always reports, even with no time elapsed
        { 0, 1, 0, -1, 200, true },        // a single-iteration run is also the final one
        { 10, 100, 200, 0, 200, true },    // exactly the interval reports
        { 10, 100, 201, 0, 200, true },    // just over the interval reports
        { 10, 100, 199, 0, 200, false },   // below the interval is suppressed (not final)
        { 0, 100, 0, -1, 200, false },     // first check at 0ms waits for the interval
        { 5, 100, 199, -1, 200, true },    // first report fires once the interval elapses (lastReport = -1)
        { 11, 100, 250, 200, 200, false }, // too soon after the previous report
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShouldReport_matches_expected(int index, int count, long elapsedMs, long lastReportMs, int intervalMs, bool expected)
    {
        var actual = ProgressReportPolicy.ShouldReport(index, count, elapsedMs, lastReportMs, intervalMs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DefaultIntervalMs_is_200() =>
        Assert.Equal(200, ProgressReportPolicy.DefaultIntervalMs);

    [Fact]
    public void ShouldReport_uses_the_default_interval_when_not_specified()
    {
        // 200ms since the last report -> reports, using the default interval.
        Assert.True(ProgressReportPolicy.ShouldReport(10, 100, 200, 0));

        // 199ms since the last report -> suppressed, using the default interval.
        Assert.False(ProgressReportPolicy.ShouldReport(10, 100, 199, 0));
    }
}
