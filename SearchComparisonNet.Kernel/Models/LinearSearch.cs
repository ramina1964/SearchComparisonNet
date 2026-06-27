namespace SearchComparisonNet.Kernel.Models;

// Shares the single dataset captured by SearchBase (dataGen.Data), so LinearSearch and
// BinarySearch operate on identical data — a prerequisite for a meaningful comparison.
public sealed class LinearSearch(IDataGenerator dataGen) : SearchBase(dataGen)
{
    // Remember: The array is sorted ascendingly
    public override ISearchItem FindItem(int value)
    {
        int? targetIndex = null;
        var noOfIterations = 0;

        for (var i = 0; i < NoOfEntries; i++)
        {
            noOfIterations++;
            if (Data[i] < value)
            {
                continue;
            }

            if (Data[i] > value)
            {
                break;
            }

            targetIndex = i;
            break;
        }

        return new SearchItem
        {
            TargetIndex = targetIndex,
            TargetValue = value,
            NoOfIterations = noOfIterations
        };
    }
}
