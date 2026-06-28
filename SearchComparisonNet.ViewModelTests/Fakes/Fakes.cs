namespace SearchComparisonNet.ViewModelTests.Fakes;

// A fixed search result. NoOfIterations feeds the average computation in the view model,
// so tests can assert the exact averages a run produces.
public sealed class FakeSearchItem : ISearchItem
{
    public int? TargetIndex { get; set; }

    public int TargetValue { get; set; }

    public int NoOfIterations { get; set; }
}

// Deterministic ISearch: NextRandomNo returns a constant, and FindItem always returns the
// same configured item. This removes randomness and timing from the simulation loop so the
// view model's command can be exercised quickly and repeatably.
public sealed class FakeSearch : ISearch
{
    private readonly int _nextValue;
    private readonly FakeSearchItem _item;

    public FakeSearch(int? targetIndex = 0, int targetValue = 0, int noOfIterations = 1, int nextValue = 0)
    {
        _nextValue = nextValue;
        _item = new FakeSearchItem
        {
            TargetIndex = targetIndex,
            TargetValue = targetValue,
            NoOfIterations = noOfIterations
        };
    }

    public int NoOfEntries { get; set; }

    public Func<int> NextRandomNo => () => _nextValue;

    public int this[int index]
    {
        get => 0;
        set { }
    }

    // Record the last value searched so target-lookup tests can assert what the view model passed in.
    public int? LastSearchedValue { get; private set; }

    // Optional hook invoked on every FindItem call. Cancellation tests use this to request
    // cancellation from inside the running simulation loop, making the cancel path deterministic.
    public Action? OnFindItem { get; set; }

    public ISearchItem FindItem(int value)
    {
        LastSearchedValue = value;
        OnFindItem?.Invoke();
        return _item;
    }
}

// Pairs a linear and a binary fake over a "shared" dataset, matching ISearchComparison.
public sealed class FakeSearchComparison : ISearchComparison
{
    public FakeSearchComparison(ISearch linearSearch, ISearch binarySearch)
    {
        LinearSearch = linearSearch;
        BinarySearch = binarySearch;
    }

    public ISearch LinearSearch { get; }

    public ISearch BinarySearch { get; }
}

// Deterministic factory: returns a preconfigured comparison and records the noOfEntries it was
// asked to build, so simulation tests can assert the view model forwarded NoOfEntries correctly.
public sealed class FakeSearchComparisonFactory : ISearchComparisonFactory
{
    private readonly ISearchComparison _comparison;

    public FakeSearchComparisonFactory(ISearchComparison? comparison = null) =>
        _comparison = comparison ?? new FakeSearchComparison(new FakeSearch(), new FakeSearch());

    public int CreateCallCount { get; private set; }

    public int? LastNoOfEntries { get; private set; }

    public ISearchComparison Create(int noOfEntries)
    {
        CreateCallCount++;
        LastNoOfEntries = noOfEntries;
        return _comparison;
    }
}
