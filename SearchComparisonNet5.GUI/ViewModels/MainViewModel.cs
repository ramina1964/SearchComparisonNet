namespace SearchComparisonNet.GUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        SimulateCommand = new RelayCommand(Simulate, CanSimulate);
        CancelCommand = new RelayCommand(Cancel, CanCancel);

        InputValidation = new InputValidation() { ClassLevelCascadeMode = CascadeMode.Stop };
        NoOfEntriesText = ProblemConstants.InitialNoOfEntries.ToString();
        NoOfSearchesText = ProblemConstants.InitialNoOfSearches.ToString();

        IsSimulating = false;
        IsSearchEnabled = false;
        ProgressBarVisibility = Visibility.Hidden;
    }

    /************************************ Public Attributes ************************************/
    public RelayCommand SimulateCommand { get; set; }

    public RelayCommand CancelCommand { get; set; }

    public InputValidation InputValidation { get; set; }

    public ISearchItem SearchItem { get; set; }

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

    public string ProgressBarLabel { get; set; }

    public ISimulationResults LinearSearchResults { get; set; }

    public ISimulationResults BinarySearchResults { get; set; }

    public string NoOfEntriesText
    {
        get => _noOfEntriesText;
        set
        {
            _ = SetProperty(ref _noOfEntriesText, value);
            var properties = new[] { nameof(NoOfEntriesText) };
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
            var properties = new[] { nameof(NoOfSearchesText) };
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
                TargetIndex = searchItem?.TargetIndex ?? null;
            }
        }
    }

    public int? TargetIndex
    {
        get => _targetIndex;
        set => SetProperty(ref _targetIndex, value);
    }

    public string Entries { get; set; }

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

    private async void Simulate()
    {
        IsSearchEnabled = false;
        TargetValue = null;
        TargetIndex = null;

        var dataParams = new DataParameters(NoOfEntries);
        var dataGen = new DataGenerator(dataParams);

        LinearSearch = new LinearSearch(dataGen);
        BinarySearch = new BinarySearch(dataGen);

        IsSimulating = true;
        LinearSearchResults = await SimulateLinearSearchAsync();
        BinarySearchResults = await SimulateBinarySearchAsync();

        LinearAvgNoOfIterations = LinearSearchResults.AvgNoOfIterations;
        LinearAvgElapsedTime = LinearSearchResults.AvgElapsedTime;

        BinaryAvgNoOfIterations = BinarySearchResults.AvgNoOfIterations;
        BinaryAvgElapsedTime = BinarySearchResults.AvgElapsedTime;
        IsSimulating = false;
        IsSearchEnabled = true;
    }

    private void Cancel() { }

    private Task<ISimulationResults> SimulateLinearSearchAsync()
    {
        return Task.Factory.StartNew(() =>
        {
            ProgressBarVisibility = Visibility.Visible;
            var totalNoOfIterations = 0.0;
            var stopwatch = Stopwatch.StartNew();
            for (var j = 0; j < NoOfSearches; j++)
            {
                var value = LinearSearch.NextRandomNo();
                var searchItem = LinearSearch.FindItem(value);
                totalNoOfIterations += searchItem.NoOfIterations;
                ProgressBarValue = (j + 1) * 100.0 / NoOfSearches;
            }
            stopwatch.Stop();
            var timeInSec = (double)stopwatch.ElapsedMilliseconds / 1000;
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;

            var elapsedTimeInSec = Math.Round(timeInSec, 1);
            return SimulationResults(totalNoOfIterations, elapsedTimeInSec);
        });
    }

    private Task<ISimulationResults> SimulateBinarySearchAsync()
    {
        return Task.Factory.StartNew(() =>
        {
            var totalNoOfIterations = 0.0;
            var stopwatch = Stopwatch.StartNew();
            for (var j = 0; j < NoOfSearches; j++)
            {
                var value = LinearSearch.NextRandomNo();
                var searchItem = BinarySearch.FindItem(value);
                totalNoOfIterations += searchItem.NoOfIterations;
                ProgressBarValue = 100 * (j + 1) / NoOfSearches;
            }
            stopwatch.Stop();
            var timeInSec = (double)stopwatch.ElapsedMilliseconds / 1000;
            ProgressBarValue = 0;
            ProgressBarVisibility = Visibility.Hidden;

            var elapsedTimeInSec = Math.Round(timeInSec, 5);
            return SimulationResults(totalNoOfIterations, elapsedTimeInSec);
        });
    }

    private ISimulationResults SimulationResults(double totalNoOfIterations, double totalElapsedTime) =>
        new SimulationResults()
        {
            NoOfEntries = NoOfEntries,
            NoOfSearches = NoOfSearches,
            AvgNoOfIterations = totalNoOfIterations / NoOfSearches,
            AvgElapsedTime = totalElapsedTime / NoOfSearches
        };

    private string GetEntries()
    {
        if (NoOfEntries <= 32)
        {
            return GetSubString(0, NoOfEntries - 1).ToString();
        }

        const string subEtc = " ..., ";
        var mid = NoOfEntries / 2;

        var startStr = GetSubString(0, 4);
        var midStr = GetSubString(mid, mid + 2);
        var endStr = GetSubString(NoOfEntries - 2, NoOfEntries - 1);

        return startStr.Append(subEtc).Append(midStr).Append(subEtc).Append(endStr).ToString();
    }

    private StringBuilder GetSubString(int fromIndex, int toIndex)
    {
        var sb = new StringBuilder();
        for (var i = fromIndex; i < toIndex; i++)
        {
            sb.Append(BinarySearch[i] + ", ");
        }

        return (toIndex == NoOfEntries - 1)
            ? sb.Append(BinarySearch[toIndex])
            : sb.Append(BinarySearch[toIndex] + ", ");
    }

    private SearchBase LinearSearch { get; set; }

    private SearchBase BinarySearch { get; set; }

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
    private static readonly long MinProductValue = (long)1e5;
    private static readonly long MaxProductValue = (long)5e10;
    private static readonly string MaxProductError =
        $"Product of No. of searches and No. of entries must be in the interval [{MinProductValue}, {MaxProductValue}].";

    private int? _targetValue;
    private string _noOfEntriesText;
    private string _noOfSearchesText;
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
