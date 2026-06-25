namespace SearchComparisonNet.Tests;

public class BinarySearchTests : TestBase
{
    [Theory]
    [InlineData(0), InlineData(1), InlineData(10), InlineData(20), InlineData(50), InlineData(100), InlineData(1000)]
    [InlineData(2000), InlineData(3000), InlineData(4000), InlineData(5000), InlineData(6000), InlineData(7000), InlineData(8000)]
    [InlineData(9000), InlineData(10000), InlineData(100_000), InlineData(200_000), InlineData(300_000), InlineData(350_000)]
    [InlineData(400_000), InlineData(450_000), InlineData(480_000), InlineData(490_000), InlineData(499_999)]
    public void Should_find_correct_index_with_binary_search_when_value_exists(int index)
    {
        // Arrange
        var expectedIndex = index;
        var value = BinarySut[index];

        // Act
        var actualIndex = BinarySut.FindItem(value).TargetIndex;

        // Assert
        Assert.Equal(expectedIndex, actualIndex);
    }

    [Theory]
    [InlineData(-1), InlineData(-100), InlineData(int.MinValue)]
    public void Should_return_null_index_with_binary_search_when_value_not_found(int missingValue)
    {
        // Act
        var actualIndex = BinarySut.FindItem(missingValue).TargetIndex;

        // Assert
        Assert.Null(actualIndex);
    }
}
