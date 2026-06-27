namespace SearchComparisonNet.Kernel.Interfaces;

public interface ISearchComparisonFactory
{
    // Builds a fresh, runtime-sized dataset and returns the two strategies that share it.
    ISearchComparison Create(int noOfEntries);
}
