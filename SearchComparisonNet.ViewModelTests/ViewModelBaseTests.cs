namespace SearchComparisonNet.ViewModelTests;

// ViewModelBase implements INotifyDataErrorInfo as an intentional no-op: input is validated via
// FluentValidation, not per-property error collections, so it never surfaces errors. These tests
// lock in that documented contract - HasErrors stays false, GetErrors stays empty, and the
// validation flow never raises ErrorsChanged.
public class ViewModelBaseTests
{
    [Fact]
    public void New_instance_reports_no_errors()
    {
        var sut = new ViewModelBase();

        Assert.Multiple(
            () => Assert.False(sut.HasErrors),
            () => Assert.Empty(sut.GetErrors(null)));
    }

    [Theory]
    [ClassData(typeof(PropertyNameData))]
    public void GetErrors_is_empty_for_every_property(string? propertyName)
    {
        var sut = new ViewModelBase();

        Assert.Empty(sut.GetErrors(propertyName));
    }

    [Fact]
    public void Invalid_input_does_not_surface_errors_or_raise_ErrorsChanged()
    {
        var raised = false;
        var sut = ViewModelFactory.Create();
        sut.ErrorsChanged += (_, _) => raised = true;

        // Drive the validation flow with input that fails the rules; it must not flow into INotifyDataErrorInfo.
        sut.NoOfEntriesText = "not-a-number";

        Assert.Multiple(
            () => Assert.False(sut.HasErrors),
            () => Assert.Empty(sut.GetErrors(nameof(MainViewModel.NoOfEntriesText))),
            () => Assert.False(raised));
    }
}

// A representative spread of property-name arguments - including null and empty - to prove
// GetErrors is empty regardless of the requested property.
public sealed class PropertyNameData : TheoryData<string?>
{
    public PropertyNameData()
    {
        Add((string?)null);
        Add(string.Empty);
        Add(nameof(MainViewModel.NoOfEntriesText));
        Add(nameof(MainViewModel.NoOfSearchesText));
        Add("UnknownProperty");
    }
}
