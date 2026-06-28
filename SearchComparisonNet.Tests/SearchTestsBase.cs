namespace SearchComparisonNet.Tests;

// Shared theory coverage exercised against every SearchBase strategy. Concrete subclasses supply
// the system under test, and the data lists live in SearchTheoryData, so the cases are defined
// once instead of being duplicated per strategy.
public abstract class SearchTestsBase
{
    protected abstract SearchBase Sut { get; }

    [Theory]
    [ClassData(typeof(FoundIndexData))]
    public void Should_find_correct_index_when_value_exists(int index)
    {
        // Arrange
        var value = Sut[index];

        // Act
        var actualIndex = Sut.FindItem(value).TargetIndex;

        // Assert
        Assert.Equal(index, actualIndex);
    }

    [Theory]
    [ClassData(typeof(MissingValueData))]
    public void Should_return_null_index_when_value_not_found(int missingValue)
    {
        // Act
        var actualIndex = Sut.FindItem(missingValue).TargetIndex;

        // Assert
        Assert.Null(actualIndex);
    }
}
