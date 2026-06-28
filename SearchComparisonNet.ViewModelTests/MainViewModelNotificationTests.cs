namespace SearchComparisonNet.ViewModelTests;

// Verifies the [ObservableProperty]-generated setters raise PropertyChanged for the exact
// names the XAML binds to, so the source-generator refactor did not silently rename anything.
public class MainViewModelNotificationTests
{
    private static List<string?> CaptureChanges(MainViewModel sut, Action mutate)
    {
        var changed = new List<string?>();
        sut.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        mutate();
        return changed;
    }

    [Fact]
    public void IsSearchEnabled_raises_property_changed()
    {
        var sut = ViewModelFactory.Create();

        var changed = CaptureChanges(sut, () => sut.IsSearchEnabled = true);

        Assert.Contains(nameof(MainViewModel.IsSearchEnabled), changed);
    }

    [Fact]
    public void IsSimulating_raises_property_changed()
    {
        var sut = ViewModelFactory.Create();

        var changed = CaptureChanges(sut, () => sut.IsSimulating = true);

        Assert.Contains(nameof(MainViewModel.IsSimulating), changed);
    }

    [Fact]
    public void ProgressBarVisibility_raises_property_changed()
    {
        var sut = ViewModelFactory.Create();

        var changed = CaptureChanges(sut, () => sut.ProgressBarVisibility = Visibility.Visible);

        Assert.Contains(nameof(MainViewModel.ProgressBarVisibility), changed);
    }

    [Fact]
    public void TargetIndex_raises_property_changed()
    {
        var sut = ViewModelFactory.Create();

        var changed = CaptureChanges(sut, () => sut.TargetIndex = 5);

        Assert.Contains(nameof(MainViewModel.TargetIndex), changed);
    }

    [Fact]
    public void Average_statistics_raise_property_changed()
    {
        var sut = ViewModelFactory.Create();

        var changed = CaptureChanges(sut, () =>
        {
            sut.LinearAvgNoOfIterations = 1;
            sut.LinearAvgElapsedTime = 2;
            sut.BinaryAvgNoOfIterations = 3;
            sut.BinaryAvgElapsedTime = 4;
        });

        Assert.Contains(nameof(MainViewModel.LinearAvgNoOfIterations), changed);
        Assert.Contains(nameof(MainViewModel.LinearAvgElapsedTime), changed);
        Assert.Contains(nameof(MainViewModel.BinaryAvgNoOfIterations), changed);
        Assert.Contains(nameof(MainViewModel.BinaryAvgElapsedTime), changed);
    }

    [Fact]
    public void NoOfEntriesText_change_notifies_is_input_valid()
    {
        var sut = ViewModelFactory.Create();

        var changed = CaptureChanges(sut, () => sut.NoOfEntriesText = "abc");

        Assert.Contains(nameof(MainViewModel.NoOfEntriesText), changed);
        Assert.Contains("IsInputValid", changed);
    }

    [Fact]
    public void Setting_same_value_does_not_raise_property_changed()
    {
        var sut = ViewModelFactory.Create();
        sut.IsSearchEnabled = true;

        var changed = CaptureChanges(sut, () => sut.IsSearchEnabled = true);

        Assert.DoesNotContain(nameof(MainViewModel.IsSearchEnabled), changed);
    }
}
