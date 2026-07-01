namespace SearchComparisonNet.ViewModelTests;

// Focused coverage of the cancellation/retry edges around SimulateCommand that the broader
// simulation tests don't assert: Cancel is unavailable until a run is in progress, and a cancelled
// run leaves the view model ready to simulate again. Cancellation is made deterministic via the
// FakeSearch.OnFindItem hook, which requests cancellation from inside the simulation loop.
public class MainViewModelCancellationTests
{
    private static MainViewModel CreateCancellingViewModel()
    {
        var linear = new FakeSearch(noOfIterations: 1);
        var binary = new FakeSearch(noOfIterations: 1);
        var comparison = new FakeSearchComparison(linear, binary);
        var sut = ViewModelFactory.Create(out _, comparison);
        sut.NoOfSearchesText = 1_000.ToString();
        linear.OnFindItem = () => sut.SimulateCommand.Cancel();
        return sut;
    }

    [Fact]
    public void Cancel_command_is_disabled_before_any_run()
    {
        var sut = ViewModelFactory.Create();

        Assert.Multiple(
            () => Assert.False(sut.CancelCommand.CanExecute(null)),
            () => Assert.True(sut.SimulateCommand.CanExecute(null)));
    }

    [Fact]
    public async Task A_cancelled_run_returns_the_view_model_to_a_ready_state()
    {
        var sut = CreateCancellingViewModel();

        await sut.SimulateCommand.ExecuteAsync(null);

        // The run ended via cancellation, so the search stays disabled but Simulate can run again.
        Assert.Multiple(
            () => Assert.False(sut.IsSimulating),
            () => Assert.False(sut.IsSearchEnabled),
            () => Assert.True(sut.SimulateCommand.CanExecute(null)),
            () => Assert.False(sut.CancelCommand.CanExecute(null)));
    }
}
