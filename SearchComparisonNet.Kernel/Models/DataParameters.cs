namespace SearchComparisonNet.Kernel.Models;

public class DataParameters(int noOfEntries)
{
    public int NoOfEntries { get; set; } = noOfEntries;

    public int MinEntryValue => ProblemConstants.MinEntryValue;

    public int MaxEntryValue => (5 * NoOfEntries) - 1;
}
