namespace SearchComparisonNet.ViewModelTests;

// These tests pin down the TargetValue -> TargetIndex behavior, including the case where the
// refactor dropped a redundant int->string->int round-trip parse from the original setter.
// The binary search used for the lookup is only assigned while a simulation runs, so tests
// that need a non-null lookup first execute SimulateCommand against a configured fake.
public class MainViewModelTargetValueTests
{
    private static MainViewModel CreateWithBinaryTargetIndex(int? targetIndex)
    {
        var binary = new FakeSearch(targetIndex: targetIndex);
        var comparison = new FakeSearchComparison(new FakeSearch(), binary);
        return ViewModelFactory.Create(out _, comparison);
    }

    [Fact]
    public void Target_index_is_null_before_any_simulation()
    {
        var sut = ViewModelFactory.Create();

        sut.TargetValue = 123;

        // No simulation has run, so BinarySearch is null and the lookup is skipped.
        Assert.Null(sut.TargetIndex);
    }

    [Fact]
    public async Task Setting_target_value_after_simulation_resolves_target_index()
    {
        var sut = CreateWithBinaryTargetIndex(targetIndex: 7);
        await sut.SimulateCommand.ExecuteAsync(null);

        sut.TargetValue = 42;

        Assert.Equal(7, sut.TargetIndex);
    }

    [Fact]
    public async Task Setting_target_value_forwards_value_to_binary_search()
    {
        var binary = new FakeSearch(targetIndex: 3);
        var comparison = new FakeSearchComparison(new FakeSearch(), binary);
        var sut = ViewModelFactory.Create(out _, comparison);
        await sut.SimulateCommand.ExecuteAsync(null);

        sut.TargetValue = 99;

        // The exact value set on TargetValue must reach FindItem unchanged (no lossy parse).
        Assert.Equal(99, binary.LastSearchedValue);
    }

    [Fact]
    public async Task Negative_target_value_is_forwarded_unchanged()
    {
        var binary = new FakeSearch(targetIndex: null);
        var comparison = new FakeSearchComparison(new FakeSearch(), binary);
        var sut = ViewModelFactory.Create(out _, comparison);
        await sut.SimulateCommand.ExecuteAsync(null);

        sut.TargetValue = -250;

        Assert.Equal(-250, binary.LastSearchedValue);
    }

    [Fact]
    public async Task Setting_target_value_to_null_does_not_perform_lookup()
    {
        var sut = CreateWithBinaryTargetIndex(targetIndex: 5);
        await sut.SimulateCommand.ExecuteAsync(null);
        sut.TargetValue = 10;
        Assert.Equal(5, sut.TargetIndex);

        sut.TargetValue = null;

        // Clearing the value leaves the previously resolved index untouched (setter returns early).
        Assert.Equal(5, sut.TargetIndex);
        Assert.Null(sut.TargetValue);
    }
}
