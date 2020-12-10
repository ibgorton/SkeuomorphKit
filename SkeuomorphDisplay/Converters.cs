using System;
using System.Globalization;
using System.Windows.Data;

namespace SkeuomorphDisplay
{
    public abstract class SkeuoConverterBase : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseDoubleConverter : SkeuoConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(double))
                throw new InvalidOperationException("The target must be a double");

            var d = (double)value;
            if (Equals(d, 0))
                return 0;
            else
                return (double)value * -1d;
        }

    }

    public class PressedConverter : SkeuoConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? 0.99 : 1d;
    }

    public class SegmentOpacityConverter : SkeuoConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? 1.0 : 0.075;
    }

    public class SelectedOpacityConverter : SkeuoConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? 0.93 : 0.95;
    }

    public class TouchOpacityConverter : SkeuoConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? 0.5 : 0d;
    }
}