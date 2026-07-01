namespace SearchComparisonNet.Kernel.Interfaces;

public interface IDataGenerator
{
    int NoOfEntries { get; set; }

    int MinValue { get; }

    int MaxValue { get; }

    int NextRandomNo();

    int[] Data { get; }

    int[] GenerateData();
}
