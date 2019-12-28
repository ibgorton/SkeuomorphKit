using System;
using System.Globalization;
using System.Windows.Data;

namespace DigitalNumericUpdown
{
    public class SelectedOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return 0.93;
                }
            }
            return 0.95;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}