namespace SearchComparisonNet.Kernel.Models;

public sealed class SearchComparisonFactory : ISearchComparisonFactory
{
    public ISearchComparison Create(int noOfEntries)
    {
        // A single DataGenerator is shared by both strategies so they operate on an
        // identical dataset, which is a prerequisite for a meaningful comparison.
        var dataParams = new DataParameters(noOfEntries);
        var dataGen = new DataGenerator(dataParams);

        return new SearchComparison(new LinearSearch(dataGen), new BinarySearch(dataGen));
    }

    private sealed class SearchComparison(ISearch linearSearch, ISearch binarySearch) : ISearchComparison
    {
        public ISearch LinearSearch { get; } = linearSearch;

        public ISearch BinarySearch { get; } = binarySearch;
    }
}
