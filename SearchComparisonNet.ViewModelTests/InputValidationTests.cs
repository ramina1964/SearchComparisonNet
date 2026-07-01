namespace SearchComparisonNet.ViewModelTests;

// Directly exercises the FluentValidation rules declared in InputValidation against a real
// MainViewModel. Each test invalidates a single property so Validate(...) yields a clean,
// deterministic failure set. Messages and bounds come from ProblemConstants - the same source of
// truth the rules themselves reference - so the assertions track the production configuration.
public class InputValidationTests
{
    // Runs the full validator and projects failures to plain tuples so the tests never depend on
    // FluentValidation result types directly.
    private static List<(string Property, string Message)> ErrorsOf(MainViewModel viewModel) =>
        new InputValidation()
            .Validate(viewModel)
            .Errors
            .Select(failure => (Property: failure.PropertyName, Message: failure.ErrorMessage))
            .ToList();

    private static bool IsValid(MainViewModel viewModel) => new InputValidation().Validate(viewModel).IsValid;

    [Fact]
    public void Default_view_model_passes_all_rules()
    {
        var sut = ViewModelFactory.Create();

        var errors = ErrorsOf(sut);

        Assert.Multiple(
            () => Assert.True(IsValid(sut)),
            () => Assert.Empty(errors));
    }

    [Fact]
    public void Blank_entries_text_reports_the_required_message()
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfEntriesText = string.Empty;

        var errors = ErrorsOf(sut);

        Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfEntriesText)
            && e.Message == ProblemConstants.NullOrEmptyNoOfEntriesMsg);
    }

    [Fact]
    public void Blank_searches_text_reports_the_required_message()
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfSearchesText = string.Empty;

        var errors = ErrorsOf(sut);

        Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfSearchesText)
            && e.Message == ProblemConstants.NullOrEmptyNoOfSearchesMsg);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.5")]
    [InlineData("1,000")]
    public void Non_integer_entries_text_reports_the_invalid_message(string text)
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfEntriesText = text;

        var errors = ErrorsOf(sut);

        Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfEntriesText)
            && e.Message == ProblemConstants.InvalidNoOfEntriesMsg);
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData("9.9")]
    public void Non_integer_searches_text_reports_the_invalid_message(string text)
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfSearchesText = text;

        var errors = ErrorsOf(sut);

        Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfSearchesText)
            && e.Message == ProblemConstants.InvalidNoOfSearchesMsg);
    }

    [Theory]
    [ClassData(typeof(OutOfRangeEntriesData))]
    public void Out_of_range_entries_report_the_range_message(int outOfRange)
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfEntries = outOfRange;

        var errors = ErrorsOf(sut);

        Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfEntries)
            && e.Message == ProblemConstants.OutOfRangeNoOfEntriesMsg);
    }

    [Theory]
    [ClassData(typeof(InRangeEntriesData))]
    public void In_range_entries_have_no_range_error(int inRange)
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfEntries = inRange;

        var errors = ErrorsOf(sut);

        Assert.DoesNotContain(errors, e => e.Property == nameof(MainViewModel.NoOfEntries));
    }

    [Theory]
    [ClassData(typeof(OutOfRangeSearchesData))]
    public void Out_of_range_searches_report_the_range_message(int outOfRange)
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfSearches = outOfRange;

        var errors = ErrorsOf(sut);

        Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfSearches)
            && e.Message == ProblemConstants.OutOfRangeNoOfSearchesMsg);
    }

    [Theory]
    [ClassData(typeof(InRangeSearchesData))]
    public void In_range_searches_have_no_range_error(int inRange)
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfSearches = inRange;

        var errors = ErrorsOf(sut);

        Assert.DoesNotContain(errors, e => e.Property == nameof(MainViewModel.NoOfSearches));
    }

    [Fact]
    public void Multiple_invalid_inputs_report_all_relevant_messages()
    {
        var sut = ViewModelFactory.Create();
        sut.NoOfEntries = ProblemConstants.MinNoOfEntries - 1;
        sut.NoOfSearches = ProblemConstants.MaxNoOfSearches + 1;

        var errors = ErrorsOf(sut);

        Assert.Multiple(
            () => Assert.False(IsValid(sut)),
            () => Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfEntries)
                && e.Message == ProblemConstants.OutOfRangeNoOfEntriesMsg),
            () => Assert.Contains(errors, e => e.Property == nameof(MainViewModel.NoOfSearches)
                && e.Message == ProblemConstants.OutOfRangeNoOfSearchesMsg));
    }
}

// Boundary values just outside the inclusive [Min, Max] ranges, plus zero, so the InclusiveBetween
// rules must reject them. Values are pulled from ProblemConstants to stay in sync with the rules.
public sealed class OutOfRangeEntriesData : TheoryData<int>
{
    public OutOfRangeEntriesData()
    {
        Add(ProblemConstants.MinNoOfEntries - 1);
        Add(ProblemConstants.MaxNoOfEntries + 1);
        Add(0);
        Add(-1);
    }
}

public sealed class InRangeEntriesData : TheoryData<int>
{
    public InRangeEntriesData()
    {
        Add(ProblemConstants.MinNoOfEntries);
        Add(ProblemConstants.MaxNoOfEntries);
        Add(ProblemConstants.InitialNoOfEntries);
    }
}

public sealed class OutOfRangeSearchesData : TheoryData<int>
{
    public OutOfRangeSearchesData()
    {
        Add(ProblemConstants.MinNoOfSearches - 1);
        Add(ProblemConstants.MaxNoOfSearches + 1);
        Add(0);
    }
}

public sealed class InRangeSearchesData : TheoryData<int>
{
    public InRangeSearchesData()
    {
        Add(ProblemConstants.MinNoOfSearches);
        Add(ProblemConstants.MaxNoOfSearches);
        Add(ProblemConstants.InitialNoOfSearches);
    }
}
