namespace SearchComparisonNet.Tests;

// Linear and binary search must agree on WHERE a value is (or that it is absent) when they run
// over the same shared dataset, which is the point of the comparison. They are not expected to
// agree on NoOfIterations, so only TargetIndex/TargetValue are compared.
public sealed class SearchEquivalenceTests
{
    private const int NoOfEntries = 1000;

    private readonly ISearchComparison _comparison =
        new SearchComparisonFactory().Create(NoOfEntries);

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(NoOfEntries / 4)]
    [InlineData(NoOfEntries / 2)]
    [InlineData((3 * NoOfEntries) / 4)]
    [InlineData(NoOfEntries - 1)]
    public void Both_strategies_agree_for_existing_values(int index)
    {
        var value = _comparison.LinearSearch[index];

        var linear = _comparison.LinearSearch.FindItem(value);
        var binary = _comparison.BinarySearch.FindItem(value);

        Assert.Equal((int?)index, linear.TargetIndex);
        Assert.Equal(linear.TargetIndex, binary.TargetIndex);
        Assert.Equal(value, linear.TargetValue);
        Assert.Equal(value, binary.TargetValue);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(5 * NoOfEntries)] // above MaxEntryValue (5 * NoOfEntries - 1)
    public void Both_strategies_agree_that_missing_values_are_absent(int missingValue)
    {
        var linear = _comparison.LinearSearch.FindItem(missingValue);
        var binary = _comparison.BinarySearch.FindItem(missingValue);

        Assert.Null(linear.TargetIndex);
        Assert.Null(binary.TargetIndex);
    }
}
