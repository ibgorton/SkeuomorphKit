using System;
using System.Globalization;
using System.Windows.Data;

namespace SkeuomorphDisplay
{
    public class SegmentOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return 1.0;
                }
            }
            return 0.075;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}