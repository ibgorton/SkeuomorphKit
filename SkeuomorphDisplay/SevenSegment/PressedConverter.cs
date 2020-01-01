using System;
using System.Globalization;
using System.Windows.Data;

namespace DigitalNumericUpdown
{
    public class PressedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return 0.99;
                }
            }
            return 1d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}