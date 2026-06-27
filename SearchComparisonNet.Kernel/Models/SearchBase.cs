namespace SearchComparisonNet.Kernel.Models;

public abstract class SearchBase(IDataGenerator dataGen) : ISearch
{
    public int NoOfEntries { get; set; } = dataGen.NoOfEntries;

    public Func<int> NextRandomNo { get; } = dataGen.NextRandomNo;

    public int this[int index]
    {
        get => Data[index];
        set
        {
            if (index < 0 || index >= NoOfEntries)
            { throw new IndexOutOfRangeException(IndexOutOfRangeError); }

            Data[index] = value;
        }
    }

    public string IndexOutOfRangeError => $"Index must be an integer in the interval [{0}, {NoOfEntries - 1}].";

    public abstract ISearchItem FindItem(int value);

    protected ObservableCollection<int> Data = dataGen.Data;
}
