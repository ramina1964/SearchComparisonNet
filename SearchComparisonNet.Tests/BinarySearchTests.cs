namespace SearchComparisonNet.Tests;

public sealed class BinarySearchTests : SearchTestsBase
{
    protected override SearchBase Sut { get; } =
        new BinarySearch(new DataGenerator(new DataParameters(ProblemConstants.InitialNoOfEntries)));
}
