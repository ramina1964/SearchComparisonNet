namespace SearchComparisonNet.Tests;

public class TestBase
{
    public TestBase()
    {
        DataParameters = new DataParameters(ProblemConstants.InitialNoOfEntries);
        DataGenerator = new DataGenerator(DataParameters);
        LinearSut = new LinearSearch(DataGenerator);
        BinarySut = new BinarySearch(DataGenerator);
    }

    protected DataParameters DataParameters { get; }

    protected SearchBase LinearSut { get; }

    protected SearchBase BinarySut { get; }

    protected DataGenerator DataGenerator { get; }
}
