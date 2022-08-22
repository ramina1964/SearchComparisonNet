using SearchComparisonNet.Kernel.Interfaces;

namespace SearchComparisonNet.Kernel.Models;

public sealed class BinarySearch : SearchBase
{
    public BinarySearch(IDataGenerator dataGen) : base(dataGen) { }

    public override ISearchItem FindItem(int value)
    {
        const int noOfIterations = 0;
        return FindItemWithBinarySearch(0, NoOfEntries - 1, value, noOfIterations);
    }

    // Remember: The array is sorted ascendingly
    private ISearchItem FindItemWithBinarySearch(int low, int high, int value, int noOfIterations)
    {
        noOfIterations++;

        // Value is non-existant, initialize and return new SearchItem with TargetIndex = null.
        if (low > high)
        {
            return new SearchItem()
            {
                TargetIndex = null,
                TargetValue = value,
                NoOfIterations = noOfIterations
            };
        }

        // Value is found, initialize and return new SearchItem for this value.
        var mid = (low + high) / 2;
        if (Data[mid] == value)
        {
            return new SearchItem()
            {
                TargetIndex = mid,
                TargetValue = value,
                NoOfIterations = noOfIterations
            };
        }

        // Throw away half of the list based on value of Data[mid] and continue searching in the remaining list.
        return Data[mid] > value
            ? FindItemWithBinarySearch(low, mid - 1, value, noOfIterations)
            : FindItemWithBinarySearch(mid + 1, high, value, noOfIterations);
    }
}
