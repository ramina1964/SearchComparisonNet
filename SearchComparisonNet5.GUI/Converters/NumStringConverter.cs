using System;
using System.Globalization;
using System.Windows.Data;

namespace SearchComparisonNet.GUI.Converters
{
    public class NumStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const double limitAbsLog = 3;
            var isAbsLogSmall = value is double db && Math.Abs(Math.Log10(db)) < limitAbsLog;

            if (value == null)
            { return null; }
            var type = value.GetType();

            var isInt = long.TryParse(value.ToString(), out var intValue);
            if (isInt)
            {
                return isAbsLogSmall ?
                    intValue.ToString("D3", CultureInfo.InvariantCulture) :
                    intValue.ToString("G3", CultureInfo.InvariantCulture);
            }

            var isDouble = double.TryParse(value.ToString(), out var dbValue);
            if (isDouble)
            {
                return isAbsLogSmall
                    ? dbValue.ToString("G3", CultureInfo.InvariantCulture)
                    : dbValue.ToString("0.00E+00", CultureInfo.InvariantCulture);
            }

            var isLong = long.TryParse(value.ToString(), out var lngValue);
            if (isLong)
            {
                return isAbsLogSmall
                    ? lngValue.ToString("G3", CultureInfo.InvariantCulture)
                    : lngValue.ToString("0E+00", CultureInfo.InvariantCulture);
            }

            // Must be a string
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueStr = value as string;
            if (int.TryParse(valueStr, out var resInt))
                return resInt;

            if (double.TryParse(valueStr, out var resDb))
                return resDb;

            if (long.TryParse(valueStr, out var resLng))
                return resLng;

            // Must be a string
            return valueStr;
        }
    }
}
