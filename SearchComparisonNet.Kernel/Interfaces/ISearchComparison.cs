namespace SearchComparisonNet.Kernel.Interfaces;

// Holds the two search strategies that operate over a single shared dataset,
// so a comparison between them is meaningful.
public interface ISearchComparison
{
    ISearch LinearSearch { get; }

    ISearch BinarySearch { get; }

    // Draws random probe values from the shared generator that produced the dataset.
    // Probe generation is a data-generation concern, so it lives here rather than on a search.
    Func<int> NextRandomNo { get; }
}
