using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Glouton.Converters;

internal sealed class LogLevelColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LogLevel level)
        {
            return level switch
            {
                LogLevel.Error => Brushes.Red,
                LogLevel.Warning => Brushes.Orange,
                LogLevel.Information => Brushes.Blue,
                LogLevel.Debug => Brushes.Gray,
                _ => Brushes.Black
            };
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

