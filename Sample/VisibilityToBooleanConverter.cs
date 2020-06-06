using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sample
{
    /// <summary>
    /// Converter between boolean value and Visibility.
    /// Used to convert between value IsProcessing flag of binding context and value on UI.
    /// </summary>
    public class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? Visibility.Visible : Visibility.Hidden;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (Visibility)value != Visibility.Visible;
        }
    }
}
