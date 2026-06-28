namespace SearchComparisonNet.Tests.Common;

// Shared theory data so the large found-index list and the missing-value list live in one place
// instead of being duplicated across the per-strategy search test classes. The indices assume a
// dataset of ProblemConstants.InitialNoOfEntries (500,000) entries, matching the SUT in SearchTestsBase.
public sealed class FoundIndexData : TheoryData<int>
{
    public FoundIndexData()
    {
        foreach (var index in Indices)
        { Add(index); }
    }

    private static readonly int[] Indices =
    [
        0, 1, 10, 20, 50, 100, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000,
        9000, 10000, 100_000, 200_000, 300_000, 350_000, 400_000, 450_000, 480_000, 490_000, 499_999,
    ];
}

// Values guaranteed to be absent from any generated dataset (entries are non-negative).
public sealed class MissingValueData : TheoryData<int>
{
    public MissingValueData()
    {
        foreach (var value in Values)
        { Add(value); }
    }

    private static readonly int[] Values = [-1, -100, int.MinValue];
}
