namespace SearchComparisonNet.GUI.Converters;

public class NegativeConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => ReturnNegative(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => ReturnNegative(value);

    private static object ReturnNegative(object? value) => value switch
    {
        bool b => !b,
        byte n => -1 * n,
        short n => -1 * n,
        int n => -1 * n,
        long n => -1L * n,
        float n => -1f * n,
        double n => -1d * n,
        decimal n => -1m * n,
        null => throw new ArgumentNullException(nameof(value)),
        _ => throw new NotSupportedException($"Type '{value.GetType()}' is not supported by {nameof(NegativeConverter)}.")
    };

    public override object ProvideValue(IServiceProvider serviceProvider)
        => _converter ??= new NegativeConverter();

    private static NegativeConverter? _converter;
}
