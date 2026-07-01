namespace SearchComparisonNet.Kernel.Models;

public abstract class SearchBase(IDataGenerator dataGen) : ISearch
{
    // Derived from the actual dataset so it can never desync from Data. The generator produces
    // Data with exactly NoOfEntries elements, so this preserves the existing value while removing
    // the previously public setter (K-2).
    public int NoOfEntries => Data.Length;

    public int this[int index]
    {
        get => Data[index];
        set
        {
            if (index < 0 || index >= NoOfEntries)
            { throw new ArgumentOutOfRangeException(nameof(index), IndexOutOfRangeError); }

            Data[index] = value;
        }
    }

    public string IndexOutOfRangeError => $"Index must be an integer in the interval [0, {NoOfEntries - 1}].";

    public abstract ISearchItem FindItem(int value);

    protected int[] Data { get; } = dataGen.Data;
}
