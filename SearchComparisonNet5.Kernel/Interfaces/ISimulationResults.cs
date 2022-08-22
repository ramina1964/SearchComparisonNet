namespace SearchComparisonNet.Kernel.Interfaces;

public interface ISimulationResults
{
    int NoOfEntries { get; set; }

    int NoOfSearches { get; set; }

    double AvgNoOfIterations { get; set; }

    double AvgElapsedTime { get; set; }
}
