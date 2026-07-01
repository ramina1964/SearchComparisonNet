namespace SearchComparisonNet.Kernel.Models;

public sealed class BinarySearch(IDataGenerator dataGen) : SearchBase(dataGen)
{
    // Remember: The array is sorted ascendingly
    public override ISearchItem FindItem(int value)
    {
        var low = 0;
        var high = NoOfEntries - 1;
        var noOfIterations = 0;

        while (true)
        {
            noOfIterations++;

            // Value is non-existent: return a SearchItem with TargetIndex = null.
            if (low > high)
            {
                return new SearchItem
                {
                    TargetIndex = null,
                    TargetValue = value,
                    NoOfIterations = noOfIterations
                };
            }

            // Overflow-safe midpoint (avoids (low + high) overflowing for large ranges).
            var mid = low + (high - low) / 2;

            // Value is found: return a SearchItem for this value.
            if (Data[mid] == value)
            {
                return new SearchItem
                {
                    TargetIndex = mid,
                    TargetValue = value,
                    NoOfIterations = noOfIterations
                };
            }

            // Throw away half of the list based on Data[mid] and continue searching the rest.
            if (Data[mid] > value)
            {
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }
    }
}
