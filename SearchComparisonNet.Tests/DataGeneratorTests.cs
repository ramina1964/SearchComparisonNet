namespace SearchComparisonNet.Tests;

// Output-invariant coverage for the production DataGenerator. The generator uses an unseeded
// Random, so these assertions target seed-independent invariants (count, range, ordering,
// uniqueness) rather than exact values. DataParameters fixes MinValue = 0 and
// MaxValue = 5 * NoOfEntries - 1, so the sampling domain always fits NoOfEntries distinct values.
public class DataGeneratorTests
{
    private static DataGenerator CreateGenerator(int noOfEntries) =>
        new(new DataParameters(noOfEntries));

    [Theory]
    [ClassData(typeof(ValidEntryCountData))]
    public void Generates_exactly_the_requested_number_of_entries(int noOfEntries)
    {
        var generator = CreateGenerator(noOfEntries);

        Assert.Equal(noOfEntries, generator.Data.Length);
    }

    [Theory]
    [ClassData(typeof(ValidEntryCountData))]
    public void All_values_fall_within_the_sampling_range(int noOfEntries)
    {
        var generator = CreateGenerator(noOfEntries);

        // Values come from offsets in [0, domainSize), so every value is in [MinValue, MaxValue).
        Assert.All(generator.Data, value => Assert.InRange(value, generator.MinValue, generator.MaxValue - 1));
    }

    [Theory]
    [ClassData(typeof(ValidEntryCountData))]
    public void Values_are_strictly_ascending_and_unique(int noOfEntries)
    {
        var data = CreateGenerator(noOfEntries).Data;

        // Ascending + distinct-count-equals-length together imply the sequence is strictly increasing.
        Assert.Multiple(
            () => Assert.Equal(data.OrderBy(value => value).ToArray(), data),
            () => Assert.Equal(data.Length, data.Distinct().Count()));
    }

    [Fact]
    public void Single_element_dataset_has_one_in_range_value()
    {
        var generator = CreateGenerator(noOfEntries: 1);

        Assert.Multiple(
            () => Assert.Single(generator.Data),
            () => Assert.InRange(generator.Data[0], generator.MinValue, generator.MaxValue - 1));
    }

    public static TheoryData<int> RandomProbeCounts => new() { 1, 10, 1_000 };

    [Theory]
    [MemberData(nameof(RandomProbeCounts))]
    public void NextRandomNo_stays_within_the_sampling_range(int noOfEntries)
    {
        var generator = CreateGenerator(noOfEntries);

        var probes = Enumerable.Range(0, 1_000).Select(_ => generator.NextRandomNo());

        Assert.All(probes, value => Assert.InRange(value, generator.MinValue, generator.MaxValue - 1));
    }
}
