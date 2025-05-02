using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EvolUX.LogViewer.Converters
{
    public class LogLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string level)
            {
                return level.ToUpper() switch
                {
                    "DEBUG" => new SolidColorBrush(Colors.Gray),
                    "INFO" => new SolidColorBrush(Colors.Green),
                    "WARNING" => new SolidColorBrush(Colors.Orange),
                    "ERROR" => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Black)
                };
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}