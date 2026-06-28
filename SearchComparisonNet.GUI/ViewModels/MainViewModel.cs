namespace SearchComparisonNet.GUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ISearchComparisonFactory _searchComparisonFactory;

    // Minimum wall-clock interval between progress reports. Report(...) marshals to the UI thread,
    // so this bounds progress-bar updates to ~5 per second regardless of NoOfSearches.
    private const int ProgressReportIntervalMs = ProgressReportPolicy.DefaultIntervalMs;

    public MainViewModel(ISearchComparisonFactory searchComparisonFactory)
    {
        _searchComparisonFactory = searchComparisonFactory;

        InputValidation = new InputValidation() { ClassLevelCascadeMode = CascadeMode.Stop };
        NoOfEntriesText = ProblemConstants.InitialNoOfEntries.ToString();
        NoOfSearchesText = ProblemConstants.InitialNoOfSearches.ToString();

        IsSimulating = false;
        IsSearchEnabled = false;
        ProgressBarVisibility = Visibility.Hidden;
    }

    /************************************ Public Attributes ************************************/
    public InputValidation InputValidation { get; }

    public string ProgressBarLabel { get; private set; } = string.Empty;

    public ISimulationResults? LinearSearchResults { get; set; }

    public ISimulationResults? BinarySearchResults { get; set; }

    // The generated SimulateCommand is an IAsyncRelayCommand; CancelCommand forwards to its
    // Cancel() because this toolkit version exposes cancellation as a method, not a command.
    [ObservableProperty]
    private bool _isSearchEnabled;

    [ObservableProperty]
    private Visibility _progressBarVisibility;

    [ObservableProperty]
    private int _noOfEntries;

    [ObservableProperty]
    private int _noOfSearches;

    [ObservableProperty]
    private int? _targetIndex;

    [ObservableProperty]
    private double _binaryAvgNoOfIterations;

    [ObservableProperty]
    private double _binaryAvgElapsedTime;

    [ObservableProperty]
    private double _linearAvgNoOfIterations;

    [ObservableProperty]
    private double _linearAvgElapsedTime;

    [ObservableProperty]
    private bool _isSimulating;

    [ObservableProperty]
    private double _progressBarValue;

    [ObservableProperty]
    private string _noOfEntriesText = string.Empty;

    [ObservableProperty]
    private string _noOfSearchesText = string.Empty;

    [ObservableProperty]
    private int? _targetValue;

    partial void OnIsSimulatingChanged(bool value) => UpdateButtonFunctionality();

    partial void OnProgressBarValueChanged(double value)
    {
        ProgressBarLabel = Math.Round(value, 0) + "%";
        OnPropertyChanged(nameof(ProgressBarLabel));
    }

    partial void OnNoOfEntriesTextChanged(string value)
    {
        string[] properties = [nameof(NoOfEntriesText)];
        IsNoOfEntriesValid = InputValidation.Validate(this, context => context.IncludeProperties(properties)).IsValid;
        OnPropertyChanged(nameof(IsInputValid));
        UpdateButtonFunctionality();

        if (!IsNoOfEntriesValid)
        {
            IsSimulating = false;
            return;
        }

        // Here: IsNoOfEntriesValid == true;
        _ = int.TryParse(value, out int noOfEntries);
        NoOfEntries = noOfEntries;
    }

    partial void OnNoOfSearchesTextChanged(string value)
    {
        string[] properties = [nameof(NoOfSearchesText)];
        IsNoOfSearchesValid = InputValidation.Validate(this, context => context.IncludeProperties(properties)).IsValid;
        OnPropertyChanged(nameof(IsInputValid));
        UpdateButtonFunctionality();

        if (!IsNoOfSearchesValid)
        {
            IsSimulating = false;
            return;
        }

        // Here: IsNoOfSearchesValid == true;
        _ = int.TryParse(value, out int noOfSearches);
        NoOfSearches = noOfSearches;
    }

    partial void OnTargetValueChanged(int? value)
    {
        if (value == null)
        { return; }

        var searchItem = BinarySearch?.FindItem(value.Value);
        TargetIndex = searchItem?.TargetIndex;
    }

    private void UpdateButtonFunctionality()
    {
        SimulateCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    /***************************************** Private Methods *****************************************/
    private bool CanSimulate() => !IsSimulating && IsInputValid;

    private bool CanCancel() => IsSimulating;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => SimulateCommand.Cancel();

    [RelayCommand(CanExecute = nameof(CanSimulate))]
    private async Task SimulateAsync(CancellationToken token)
    {
        IsSearchEnabled = false;
        TargetValue = null;
        TargetIndex = null;

        var searchComparison = _searchComparisonFactory.Create(NoOfEntries);
        LinearSearch = searchComparison.LinearSearch;
        BinarySearch = searchComparison.BinarySearch;

        IsSimulating = true;

        // Progress is reported through IProgress<T>. Because the Progress<T> instance is
        // created here on the UI thread, its callbacks marshal back to the UI thread, so the
        // background simulation never touches UI-bound properties directly.
        var progress = new Progress<double>(value => ProgressBarValue = value);
        ProgressBarVisibility = Visibility.Visible;

        try
        {
            // Linear and binary runs differ only in how aggressively their elapsed time is
            // rounded for display (1 vs 5 fractional digits); the loop itself is identical.
            var linearResults = await RunSimulationAsync(LinearSearch!, roundDigits: 1, progress, token);
            var binaryResults = await RunSimulationAsync(BinarySearch!, roundDigits: 5, progress, token);

            LinearSearchResults = linearResults;
            BinarySearchResults = binaryResults;

            LinearAvgNoOfIterations = linearResults.AvgNoOfIterations;
            LinearAvgElapsedTime = linearResults.AvgElapsedTime;

            BinaryAvgNoOfIterations = binaryResults.AvgNoOfIterations;
            BinaryAvgElapsedTime = binaryResults.AvgElapsedTime;

            IsSearchEnabled = true;
        }
        catch (OperationCanceledException)
        {
            // Cancellation is a normal outcome, not an error: leave results as-is and keep
            // the search panel disabled since the run did not complete.
            IsSearchEnabled = false;
        }
        finally
        {
            // Always run on the UI thread (continuation after await), so resetting the
            // progress bar here is thread-safe and deterministic.
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;
            IsSimulating = false;
        }
    }

    private Task<ISimulationResults> RunSimulationAsync(ISearch search, int roundDigits, IProgress<double> progress, CancellationToken token) =>
        Task.Run(() =>
        {
            var totalNoOfIterations = 0.0;
            var stopwatch = Stopwatch.StartNew();
            var lastReportMs = -1L;
            for (var j = 0; j < NoOfSearches; j++)
            {
                token.ThrowIfCancellationRequested();
                var value = search.NextRandomNo();
                var searchItem = search.FindItem(value);
                totalNoOfIterations += searchItem.NoOfIterations;
                // Throttle progress by elapsed time: Report(...) marshals to the UI thread, so
                // updating every iteration floods the dispatcher. Reporting at most once per
                // ProgressReportIntervalMs keeps UI updates bounded and adapts to the run length;
                // the final iteration always reports so the bar reaches 100%.
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                if (ProgressReportPolicy.ShouldReport(j, NoOfSearches, elapsedMs, lastReportMs, ProgressReportIntervalMs))
                {
                    lastReportMs = elapsedMs;
                    progress.Report((j + 1) * 100.0 / NoOfSearches);
                }
            }
            stopwatch.Stop();
            var timeInSec = (double)stopwatch.ElapsedMilliseconds / 1000;

            var elapsedTimeInSec = Math.Round(timeInSec, roundDigits);
            return SimulationResults(totalNoOfIterations, elapsedTimeInSec);
        }, token);

    private ISimulationResults SimulationResults(double totalNoOfIterations, double totalElapsedTime) =>
        new SimulationResults()
        {
            NoOfEntries = NoOfEntries,
            NoOfSearches = NoOfSearches,
            AvgNoOfIterations = totalNoOfIterations / NoOfSearches,
            AvgElapsedTime = totalElapsedTime / NoOfSearches
        };

    private ISearch? LinearSearch { get; set; }

    private ISearch? BinarySearch { get; set; }

    [ObservableProperty]
    private bool _isNoOfEntriesValid;

    [ObservableProperty]
    private bool _isNoOfSearchesValid;

    private bool IsInputValid => IsNoOfEntriesValid && IsNoOfSearchesValid;
}
