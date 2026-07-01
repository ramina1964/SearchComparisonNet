namespace SearchComparisonNet.Kernel.Interfaces;

public interface ISearch
{
    int NoOfEntries { get; }

    Func<int> NextRandomNo { get; }

    int this[int index] { get; set; }

    ISearchItem FindItem(int value);
}
