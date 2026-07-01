namespace SearchComparisonNet.Kernel.Models;

public class DataParameters(int noOfEntries)
{
    public int NoOfEntries { get; set; } = noOfEntries;

    // Kept as an instance member (not static) to mirror MaxEntryValue, which is instance-dependent,
    // and because callers access it through a DataParameters instance (e.g. dataParams.MinEntryValue).
    public int MinEntryValue => ProblemConstants.MinEntryValue;

    public int MaxEntryValue => (5 * NoOfEntries) - 1;
}
