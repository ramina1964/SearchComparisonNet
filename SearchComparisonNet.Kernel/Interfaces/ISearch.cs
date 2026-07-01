namespace SearchComparisonNet.Kernel.Interfaces;

public interface ISearch
{
    int NoOfEntries { get; }

    int this[int index] { get; set; }

    ISearchItem FindItem(int value);
}
