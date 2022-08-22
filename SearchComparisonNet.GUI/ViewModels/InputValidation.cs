namespace SearchComparisonNet.GUI.ViewModels;

public class InputValidation : AbstractValidator<MainViewModel>
{
    #region Constructor

    public InputValidation() => ValidationRules();

    #endregion Constructor

    private void ValidationRules()
    {
        // NoOfEntriesText
        _ = RuleFor(vm => vm.NoOfEntriesText)
            .NotNull().NotEmpty()
            .WithMessage(ProblemConstants.NullOrEmptyNoOfEntriesMsg)
            .Must(noOfEntriesText => int.TryParse(noOfEntriesText, out var noOfEntries))
            .WithMessage(ProblemConstants.InvalidNoOfEntriesMsg);

        // NoOfEntries
        _ = RuleFor(vm => vm.NoOfEntries)
            .InclusiveBetween(ProblemConstants.MinNoOfEntries, ProblemConstants.MaxNoOfEntries)
            .WithMessage(ProblemConstants.OutOfRangeNoOfEntriesMsg);

        // NoOfSearchesText
        _ = RuleFor(vm => vm.NoOfSearchesText)
           .NotNull().NotEmpty()
           .WithMessage(ProblemConstants.NullOrEmptyNoOfSearchesMsg)
           .Must(noOfSearchesText => int.TryParse(noOfSearchesText, out var noOfSearches))
           .WithMessage(ProblemConstants.InvalidNoOfSearchesMsg);

        // NoOfSearches
        _ = RuleFor(vm => vm.NoOfSearches)
            .InclusiveBetween(ProblemConstants.MinNoOfSearches, ProblemConstants.MaxNoOfSearches)
            .WithMessage(ProblemConstants.OutOfRangeNoOfSearchesMsg);
    }
}
