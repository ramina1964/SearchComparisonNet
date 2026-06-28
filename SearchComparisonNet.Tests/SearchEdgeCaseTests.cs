namespace SearchComparisonNet.Tests;

// Deterministic boundary/edge coverage for both search strategies over hand-crafted datasets
// (via FakeDataGenerator), independent of the random production DataGenerator. Running the same
// cases through both strategies also confirms they agree on these inputs.
public sealed class SearchEdgeCaseTests
{
    public static TheoryData<int[], int, int?> Cases => new()
    {
        { new int[] { }, 5, null },                 // empty dataset
        { new[] { 42 }, 42, 0 },                    // single element, found
        { new[] { 42 }, 7, null },                  // single element, missing
        { new[] { 10, 20, 30, 40, 50 }, 10, 0 },    // first element
        { new[] { 10, 20, 30, 40, 50 }, 50, 4 },    // last element
        { new[] { 10, 20, 30, 40, 50 }, 30, 2 },    // middle element
        { new[] { 10, 20, 30, 40, 50 }, 25, null }, // gap between entries
        { new[] { 10, 20, 30, 40, 50 }, 5, null },  // below the smallest entry
        { new[] { 10, 20, 30, 40, 50 }, 99, null }, // above the largest entry
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void Linear_search_returns_expected_index(int[] data, int target, int? expectedIndex)
    {
        var sut = new LinearSearch(new FakeDataGenerator(data));

        var actualIndex = sut.FindItem(target).TargetIndex;

        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void Binary_search_returns_expected_index(int[] data, int target, int? expectedIndex)
    {
        var sut = new BinarySearch(new FakeDataGenerator(data));

        var actualIndex = sut.FindItem(target).TargetIndex;

        Assert.Equal(expectedIndex, actualIndex);
    }
}
