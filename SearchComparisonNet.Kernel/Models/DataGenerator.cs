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

    public ObservableCollection<int> Data { get; }

    public Random Random { get; }

    #region IDataGenerator
    public int NoOfEntries { get; set; }

    public int MinValue { get; }

    public int MaxValue { get; }

    public int NextRandomNo() => Random.Next(MinValue, MaxValue);

    public ObservableCollection<int> GenerateData()
    {
        var data = new HashSet<int>();

        while (data.Count < NoOfEntries)
        { data.Add(Random.Next(MinValue, MaxValue)); }

        var result = data.ToList();
        result.Sort();
        return new ObservableCollection<int>(result);
    }
    #endregion IDataGenerator
}
