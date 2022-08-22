namespace SearchComparisonNet.Kernel.Models;

public class DataParameters
{
    public DataParameters(int noOfEntries)
    {
        NoOfEntries = noOfEntries;
    }

    public int NoOfEntries { get; set; }

    public int MinEntryValue => ProblemConstants.MinEntryValue;

    public int MaxEntryValue => (5 * NoOfEntries) - 1;
}
