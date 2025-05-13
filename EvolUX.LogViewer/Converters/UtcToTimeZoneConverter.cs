using System;
using System.Globalization;
using System.Windows.Data;

namespace EvolUX.LogViewer.Converters
{
    public class UtcToTimeZoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Convert UTC time to local time
                DateTime localTime;

                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    localTime = dateTime.ToLocalTime();
                }
                else
                {
                    // If not explicitly UTC, assume it is and convert
                    localTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
                }

                // If a format string is provided as a parameter
                if (parameter is string format)
                {
                    return localTime.ToString(format);
                }

                return localTime;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}