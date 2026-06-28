namespace SearchComparisonNet.Kernel.Models;

public class DataGenerator : IDataGenerator
{
    public DataGenerator(DataParameters dataParams)
    {
        Random = new Random();
        NoOfEntries = dataParams.NoOfEntries;
        MinValue = dataParams.MinEntryValue;
        MaxValue = dataParams.MaxEntryValue;
        Data = GenerateData();
    }

    public int[] Data { get; }

    public Random Random { get; }

    #region IDataGenerator
    public int NoOfEntries { get; set; }

    public int MinValue { get; }

    public int MaxValue { get; }

    public int NextRandomNo() => Random.Next(MinValue, MaxValue);

    public int[] GenerateData()
    {
        var data = new HashSet<int>(NoOfEntries);

        while (data.Count < NoOfEntries)
        { data.Add(Random.Next(MinValue, MaxValue)); }

        var sorted = new int[NoOfEntries];
        data.CopyTo(sorted);
        Array.Sort(sorted);
        return sorted;
    }
    #endregion IDataGenerator
}
