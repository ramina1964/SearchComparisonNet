namespace SearchComparisonNet.Kernel.Models;

public abstract class SearchBase
{
    protected SearchBase(IDataGenerator dataGen)
    {
        NoOfEntries = dataGen.NoOfEntries;
        NextRandomNo = dataGen.NextRandomNo;
        Data = dataGen.Data;
    }

    public int NoOfEntries { get; set; }

    public Func<int> NextRandomNo { get; }

    public int this[int index]
    {
        get => Data[index];
        set
        {
            if (0 > index || index > NoOfEntries - 1)
            { throw new IndexOutOfRangeException(IndexOutOfRangeError); }

            Data[index] = value;
        }
    }

    public string IndexOutOfRangeError => $"Index must be an integer in the interval [{0}, {NoOfEntries - 1}].";

    public abstract ISearchItem FindItem(int value);

    protected ObservableCollection<int> Data;
}
