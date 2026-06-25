namespace SearchComparisonNet.Kernel.Models;

public static class ProblemConstants
{
    // No of Entries
    // NOTE: value preserved exactly (10,000). Flagged in review (K-3) as a possible typo for 100_000 — left unchanged per decision.
    public static int MinNoOfEntries => 10_000;

    public static int InitialNoOfEntries => 500_000;

    public static int MaxNoOfEntries => 50_000_000;

    // Entry Values 
    public static int MinEntryValue => 0;

    // No of Searches
    public static int MinNoOfSearches => 1_000;

    public static int InitialNoOfSearches => 5_000;

    public static int MaxNoOfSearches => 500_000;

    public static string NullOrEmptyNoOfEntriesMsg => "NoOfEntriesText is a required field.";

    public static string NullOrEmptyNoOfSearchesMsg => $"NoOfSearchesText is a required field.";

    public static string InvalidNoOfEntriesMsg => "NoOfEntriesText must be a valid integer.";

    public static string InvalidNoOfSearchesMsg => "NoOfSearchesText must be a valid integer.";

    public static string OutOfRangeNoOfEntriesMsg => "NoOfEntriesText must be an integer in the " +
                                                     $"interval [{MinNoOfEntries}, {MaxNoOfEntries}].";

    public static string OutOfRangeNoOfSearchesMsg => "NoOfSearchesText must be an integer in the " +
                                                      $"interval [{MinNoOfSearches}, {MaxNoOfSearches}].";
}
