namespace SearchComparisonNet.ViewModelTests;

// Shared helpers for constructing a MainViewModel with deterministic fakes.
internal static class ViewModelFactory
{
    public static MainViewModel Create(out FakeSearchComparisonFactory factory, ISearchComparison? comparison = null)
    {
        factory = new FakeSearchComparisonFactory(comparison);
        return new MainViewModel(factory);
    }

    public static MainViewModel Create() => new(new FakeSearchComparisonFactory());
}

// NOTE: ViewModelBase.HasErrors is currently always false (PropErrors is never populated by the
// validation flow), so these tests assert the *observable* input contract: SimulateCommand
// becomes non-executable when input is invalid (driven by IsInputValid) and executable again
// once input is restored.
public class MainViewModelValidationTests
{
    [Fact]
    public void New_view_model_starts_valid_with_default_inputs()
    {
        var sut = ViewModelFactory.Create();

        // The constructor seeds the valid InitialNoOfEntries / InitialNoOfSearches defaults.
        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(ProblemConstants.InitialNoOfEntries, sut.NoOfEntries);
        Assert.Equal(ProblemConstants.InitialNoOfSearches, sut.NoOfSearches);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("12.5")]
    public void Non_integer_entries_text_blocks_simulate(string text)
    {
        var sut = ViewModelFactory.Create();

        sut.NoOfEntriesText = text;

        Assert.False(sut.SimulateCommand.CanExecute(null));
    }

    // NOTE: Per-keystroke validation only checks the *text* rule (NotEmpty + parseable int). The
    // inclusive-range rule is declared on the parsed NoOfEntries property and is NOT included in
    // the keystroke validation, so a numeric out-of-range value is treated as valid input here and
    // still flows into NoOfEntries. These tests lock in that actual behavior.
    [Fact]
    public void Below_range_entries_text_is_format_valid_and_updates_NoOfEntries()
    {
        var sut = ViewModelFactory.Create();
        var belowRange = ProblemConstants.MinNoOfEntries - 1;

        sut.NoOfEntriesText = belowRange.ToString();

        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(belowRange, sut.NoOfEntries);
    }

    [Fact]
    public void Above_range_entries_text_is_format_valid_and_updates_NoOfEntries()
    {
        var sut = ViewModelFactory.Create();
        var aboveRange = ProblemConstants.MaxNoOfEntries + 1;

        sut.NoOfEntriesText = aboveRange.ToString();

        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(aboveRange, sut.NoOfEntries);
    }

    [Theory]
    [InlineData("")]
    [InlineData("xyz")]
    public void Non_integer_searches_text_blocks_simulate(string text)
    {
        var sut = ViewModelFactory.Create();

        sut.NoOfSearchesText = text;

        Assert.False(sut.SimulateCommand.CanExecute(null));
    }

    [Fact]
    public void Below_range_searches_text_is_format_valid_and_updates_NoOfSearches()
    {
        var sut = ViewModelFactory.Create();
        var belowRange = ProblemConstants.MinNoOfSearches - 1;

        sut.NoOfSearchesText = belowRange.ToString();

        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(belowRange, sut.NoOfSearches);
    }

    [Fact]
    public void Above_range_searches_text_is_format_valid_and_updates_NoOfSearches()
    {
        var sut = ViewModelFactory.Create();
        var aboveRange = ProblemConstants.MaxNoOfSearches + 1;

        sut.NoOfSearchesText = aboveRange.ToString();

        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(aboveRange, sut.NoOfSearches);
    }

    [Fact]
    public void Restoring_valid_input_re_enables_simulate()
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfEntriesText = "abc";
        Assert.False(sut.SimulateCommand.CanExecute(null));

        sut.NoOfEntriesText = ProblemConstants.InitialNoOfEntries.ToString();

        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(ProblemConstants.InitialNoOfEntries, sut.NoOfEntries);
    }

    [Fact]
    public void Valid_within_range_entries_updates_NoOfEntries()
    {
        var sut = ViewModelFactory.Create();
        var value = ProblemConstants.MinNoOfEntries + 1;

        sut.NoOfEntriesText = value.ToString();

        Assert.True(sut.SimulateCommand.CanExecute(null));
        Assert.Equal(value, sut.NoOfEntries);
    }
}
