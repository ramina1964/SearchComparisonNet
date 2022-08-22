using SearchComparisonNet.Kernel.Interfaces;

namespace SearchComparisonNet.Kernel.Models;

public class SearchItem : ISearchItem
{
    public int? TargetIndex { get; set; }

    public int TargetValue { get; set; }

    public int NoOfIterations { get; set; }
}
