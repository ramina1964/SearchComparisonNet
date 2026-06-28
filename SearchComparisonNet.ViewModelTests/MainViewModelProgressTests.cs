namespace SearchComparisonNet.ViewModelTests;

public class MainViewModelProgressTests
{
    [Fact]
    public void Progress_bar_is_hidden_initially()
    {
        var sut = ViewModelFactory.Create();

        Assert.Equal(Visibility.Hidden, sut.ProgressBarVisibility);
    }

    // NOTE: 0.0 is excluded here because the backing field already defaults to 0.0, so assigning 0.0
    // to a fresh view model is a no-op (the generated setter suppresses equal values) and the label
    // formatter never runs. The "0%" formatting is covered by Progress_label_formats_zero_after_change.
    [Theory]
    [InlineData(50.0, "50%")]
    [InlineData(100.0, "100%")]
    public void Setting_progress_value_formats_whole_percent_label(double value, string expected)
    {
        var sut = ViewModelFactory.Create();

        sut.ProgressBarValue = value;

        Assert.Equal(expected, sut.ProgressBarLabel);
    }

    [Fact]
    public void Progress_label_formats_zero_after_change()
    {
        var sut = ViewModelFactory.Create();

        // Move away from the default 0 first so that setting it back to 0 is an actual change and
        // the label formatter runs, producing "0%".
        sut.ProgressBarValue = 25.0;
        sut.ProgressBarValue = 0.0;

        Assert.Equal("0%", sut.ProgressBarLabel);
    }

    [Theory]
    [InlineData(49.4, "49%")]
    [InlineData(49.5, "50%")]   // Math.Round uses banker's rounding: 49.5 -> 50 (nearest even is 50)
    [InlineData(50.5, "50%")]   // banker's rounding: 50.5 -> 50 (nearest even)
    [InlineData(2.5, "2%")]     // banker's rounding: 2.5 -> 2 (nearest even)
    public void Progress_label_rounds_to_nearest_whole_percent(double value, string expected)
    {
        var sut = ViewModelFactory.Create();

        sut.ProgressBarValue = value;

        Assert.Equal(expected, sut.ProgressBarLabel);
    }

    [Fact]
    public void Setting_progress_value_raises_property_changed_for_value_and_label()
    {
        var sut = ViewModelFactory.Create();
        var changed = new List<string?>();
        sut.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        sut.ProgressBarValue = 42;

        Assert.Contains(nameof(MainViewModel.ProgressBarValue), changed);
        Assert.Contains(nameof(MainViewModel.ProgressBarLabel), changed);
    }
}
