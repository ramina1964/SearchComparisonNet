namespace SearchComparisonNet.Kernel.Interfaces;

// Holds the two search strategies that operate over a single shared dataset,
// so a comparison between them is meaningful.
public interface ISearchComparison
{
    ISearch LinearSearch { get; }

    ISearch BinarySearch { get; }
}
