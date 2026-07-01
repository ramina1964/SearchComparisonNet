namespace SearchComparisonNet.Tests.Common;

// Shared entry counts for DataGenerator output tests. Each value is a valid NoOfEntries: the
// generator's sampling domain is [0, 5 * NoOfEntries - 1) (see DataParameters), which always has
// room for NoOfEntries distinct values, so these counts never risk an exhausted-domain loop.
public sealed class ValidEntryCountData : TheoryData<int>
{
    public ValidEntryCountData()
    {
        foreach (var count in Counts)
        { Add(count); }
    }

    private static readonly int[] Counts = [1, 2, 5, 100, 1_000, 5_000];
}
