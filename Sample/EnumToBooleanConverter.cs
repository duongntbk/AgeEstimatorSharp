using System;
using System.Globalization;
using System.Windows.Data;

namespace Sample
{
    /// <summary>
    /// Converter between boolean value and enum.
    /// Used to convert between value of radio button on UI and AnnotationOption/FaceDetectorOption enum.
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}