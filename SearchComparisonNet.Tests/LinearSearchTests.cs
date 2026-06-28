namespace SearchComparisonNet.Tests;

public sealed class LinearSearchTests : SearchTestsBase
{
    protected override SearchBase Sut { get; } =
        new LinearSearch(new DataGenerator(new DataParameters(ProblemConstants.InitialNoOfEntries)));
}
