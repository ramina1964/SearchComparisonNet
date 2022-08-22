namespace SearchComparisonNet.Kernel.Models;

//Todo: Both LinearSearch and BinarySerch must be run for TargetValue 0 after each simulation as well as updating the TargetValue textbox with the result.
public sealed class LinearSearch : SearchBase
{
    public LinearSearch(IDataGenerator dataGen) : base(dataGen)
    {
        NoOfEntries = dataGen.NoOfEntries;
        Data = dataGen.GenerateData();
    }

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
