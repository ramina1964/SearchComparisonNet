namespace SearchComparisonNet.GUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        SimulateCommand = new AsyncRelayCommand(SimulateAsync, CanSimulate);
        // AsyncRelayCommand exposes cancellation as a Cancel() method (not a command in this
        // toolkit version), so Cancel just forwards to it. CanCancel keeps the button enabled
        // only while a simulation is actually running.
        CancelCommand = new RelayCommand(Cancel, CanCancel);

        InputValidation = new InputValidation() { ClassLevelCascadeMode = CascadeMode.Stop };
        NoOfEntriesText = ProblemConstants.InitialNoOfEntries.ToString();
        NoOfSearchesText = ProblemConstants.InitialNoOfSearches.ToString();

        IsSimulating = false;
        IsSearchEnabled = false;
        ProgressBarVisibility = Visibility.Hidden;
    }

    /************************************ Public Attributes ************************************/
    public IAsyncRelayCommand SimulateCommand { get; }

    public RelayCommand CancelCommand { get; }

    public InputValidation InputValidation { get; }

    public bool IsSimulating
    {
        get => _isSimulating;
        set
        {
            if (SetProperty(ref _isSimulating, value))
            {
                UpdateButtonFunctionality();
            }
        }
    }

    public bool IsSearchEnabled
    {
        get => _isSearchEnabled;
        set => SetProperty(ref _isSearchEnabled, value);
    }

    private void UpdateButtonFunctionality()
    {
        SimulateCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    public Visibility ProgressBarVisibility
    {
        get => _progressBarVisibility;
        set => SetProperty(ref _progressBarVisibility, value);
    }

    public double ProgressBarValue
    {
        get => _progressBarValue;
        set
        {
            _ = SetProperty(ref _progressBarValue, value);
            ProgressBarLabel = Math.Round(value, 0) + "%";
            OnPropertyChanged(nameof(ProgressBarLabel));
        }
    }

    public string ProgressBarLabel { get; set; } = string.Empty;

    public ISimulationResults? LinearSearchResults { get; set; }

    public ISimulationResults? BinarySearchResults { get; set; }

    public string NoOfEntriesText
    {
        get => _noOfEntriesText;
        set
        {
            _ = SetProperty(ref _noOfEntriesText, value);
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
    }

    public string NoOfSearchesText
    {
        get => _noOfSearchesText;
        set
        {
            _ = SetProperty(ref _noOfSearchesText, value);
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
    }

    public int NoOfEntries
    {
        get => _noOfEntries;
        set => SetProperty(ref _noOfEntries, value);
    }

    public int NoOfSearches
    {
        get => _noOfSearches;
        set => SetProperty(ref _noOfSearches, value);
    }

    public int? TargetValue
    {
        get => _targetValue;
        set
        {
            if (value == null)
            {
                SetProperty(ref _targetValue, null);
                return;
            }

            var valid = int.TryParse(value.Value.ToString(), out int result);
            if (!valid)
            { return; }

            if (SetProperty(ref _targetValue, result))
            {
                var searchItem = BinarySearch?.FindItem(value.Value);
                TargetIndex = searchItem?.TargetIndex;
            }
        }
    }

    public int? TargetIndex
    {
        get => _targetIndex;
        set => SetProperty(ref _targetIndex, value);
    }

    public double BinaryAvgNoOfIterations
    {
        get => _binaryAvgNoOfIterations;
        set => SetProperty(ref _binaryAvgNoOfIterations, value);
    }

    public double BinaryAvgElapsedTime
    {
        get => _binaryAvgElapsedTime;
        set => SetProperty(ref _binaryAvgElapsedTime, value);
    }

    public double LinearAvgNoOfIterations
    {
        get => _linearAvgNoOfIterations;
        set => SetProperty(ref _linearAvgNoOfIterations, value);
    }

    public double LinearAvgElapsedTime
    {
        get => _linearAvgElapsedTime;
        set => SetProperty(ref _linearAvgElapsedTime, value);
    }

    /***************************************** Private Methods *****************************************/
    private bool CanSimulate() => !IsSimulating && IsInputValid;

    private bool CanCancel() => IsSimulating;

    private void Cancel() => SimulateCommand.Cancel();

    private async Task SimulateAsync(CancellationToken token)
    {
        IsSearchEnabled = false;
        TargetValue = null;
        TargetIndex = null;

        var dataParams = new DataParameters(NoOfEntries);
        var dataGen = new DataGenerator(dataParams);

        LinearSearch = new LinearSearch(dataGen);
        BinarySearch = new BinarySearch(dataGen);

        IsSimulating = true;

        // Progress is reported through IProgress<T>. Because the Progress<T> instance is
        // created here on the UI thread, its callbacks marshal back to the UI thread, so the
        // background simulation never touches UI-bound properties directly.
        var progress = new Progress<double>(value => ProgressBarValue = value);
        ProgressBarVisibility = Visibility.Visible;

        try
        {
            var linearResults = await SimulateLinearSearchAsync(progress, token);
            var binaryResults = await SimulateBinarySearchAsync(progress, token);

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

    private Task<ISimulationResults> SimulateLinearSearchAsync(IProgress<double> progress, CancellationToken token)
    {
        var search = LinearSearch!;
        return Task.Run(() =>
        {
            var totalNoOfIterations = 0.0;
            var stopwatch = Stopwatch.StartNew();
            for (var j = 0; j < NoOfSearches; j++)
            {
                token.ThrowIfCancellationRequested();
                var value = search.NextRandomNo();
                var searchItem = search.FindItem(value);
                totalNoOfIterations += searchItem.NoOfIterations;
                progress.Report((j + 1) * 100.0 / NoOfSearches);
            }
            stopwatch.Stop();
            var timeInSec = (double)stopwatch.ElapsedMilliseconds / 1000;

            var elapsedTimeInSec = Math.Round(timeInSec, 1);
            return SimulationResults(totalNoOfIterations, elapsedTimeInSec);
        }, token);
    }

    private Task<ISimulationResults> SimulateBinarySearchAsync(IProgress<double> progress, CancellationToken token)
    {
        var search = BinarySearch!;
        return Task.Run(() =>
        {
            var totalNoOfIterations = 0.0;
            var stopwatch = Stopwatch.StartNew();
            for (var j = 0; j < NoOfSearches; j++)
            {
                token.ThrowIfCancellationRequested();
                var value = search.NextRandomNo();
                var searchItem = search.FindItem(value);
                totalNoOfIterations += searchItem.NoOfIterations;
                progress.Report(100.0 * (j + 1) / NoOfSearches);
            }
            stopwatch.Stop();
            var timeInSec = (double)stopwatch.ElapsedMilliseconds / 1000;

            var elapsedTimeInSec = Math.Round(timeInSec, 5);
            return SimulationResults(totalNoOfIterations, elapsedTimeInSec);
        }, token);
    }

    private ISimulationResults SimulationResults(double totalNoOfIterations, double totalElapsedTime) =>
        new SimulationResults()
        {
            NoOfEntries = NoOfEntries,
            NoOfSearches = NoOfSearches,
            AvgNoOfIterations = totalNoOfIterations / NoOfSearches,
            AvgElapsedTime = totalElapsedTime / NoOfSearches
        };

    private SearchBase? LinearSearch { get; set; }

    private SearchBase? BinarySearch { get; set; }

    private bool IsNoOfEntriesValid
    {
        get => _isNoOfEntriesValid;
        set => SetProperty(ref _isNoOfEntriesValid, value);
    }

    private bool IsNoOfSearchesValid
    {
        get => _isNoOfSearchesValid;
        set => SetProperty(ref _isNoOfSearchesValid, value);
    }

    private bool IsInputValid => IsNoOfEntriesValid && IsNoOfSearchesValid;

    /***************************************** Private Fields ******************************************/
    private int? _targetValue;
    private string _noOfEntriesText = string.Empty;
    private string _noOfSearchesText = string.Empty;
    private int _noOfEntries;
    private int _noOfSearches;
    private bool _isSimulating;
    private Visibility _progressBarVisibility;
    private double _progressBarValue;
    private int? _targetIndex;
    private double _linearAvgNoOfIterations;
    private double _linearAvgElapsedTime;
    private double _binaryAvgNoOfIterations;
    private double _binaryAvgElapsedTime;
    private bool _isNoOfEntriesValid;
    private bool _isNoOfSearchesValid;
    private bool _isSearchEnabled;
}
