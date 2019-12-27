using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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

    /// <summary>
    /// Interaction logic for SevenSegmentModule.xaml
    /// </summary>
    public partial class SevenSegmentLED : SevenSegmentBase
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public SevenSegmentLED() : base()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            ConfigureEvents();
            ShowDigitSelector = false;
            IsSelected = false;

        }

        /// <summary>
        /// Used for seven segment illumination logic
        /// </summary>
        private readonly static BitArray[] _bits = {
                //zero
                new BitArray(new bool[] { true, true, true, false, true, true, true }),
                //one
                new BitArray(new bool[] { false, false, true, false, false, true, false }),
                //two
                new BitArray(new bool[] { true, false, true, true, true, false, true }),
                //three
                new BitArray(new bool[] { true, false, true, true, false, true, true }),
                //four
                new BitArray(new bool[] { false, true, true, true, false, true, false }),
                //five
                new BitArray(new bool[] { true, true, false, true, false, true, true }),
                //six
                new BitArray(new bool[] { true, true, false, true, true, true, true }),
                //seven
                new BitArray(new bool[] { true, false, true, false, false, true, false }),
                //eight
                new BitArray(new bool[] { true, true, true, true, true, true, true }),
                //nine
                new BitArray(new bool[] { true, true, true, true, false, true, true }),
                //null
                new BitArray(new bool[] { false, false, false, false, false, false, false }),
                //-
                new BitArray(new bool [] {false, false, false, true, false, false, false })
        };

        

        private int? _currentValue = null;

        //private
        private bool _showDigitSelector;
        

        public event Action<object> SelectionEvent = delegate { };

        

        public Brush BackgroundFill
        {
            get => (Brush)GetValue(BackgroundFillProperty);
            set => SetValue(BackgroundFillProperty, value);
        }

        public bool BottomPressed
        {
            get => (bool)GetValue(BottomPressedProperty);
            set
            {
                SetValue(BottomPressedProperty, value);
                Pressed = value;
            }
        }

        //public
        public int? CurrentValue => _currentValue;
        public double DecimalDisplayAngle
        {
            get => (double)GetValue(DecimalDisplayAngleProperty);
            set => SetValue(DecimalDisplayAngleProperty, value);
        }

        //public double Increment { get; set; } = 1.0;

        

        public double SegmentDisplayAngle
        {
            get => (double)GetValue(SegmentDisplayAngleProperty);
            set => SetValue(SegmentDisplayAngleProperty, value);
        }

        

        public bool ShowDecimalPoint
        {
            get => (bool)GetValue(ShowDecimalPointProperty);
            set => SetValue(ShowDecimalPointProperty, value);
        }

        public bool ShowDigitSelector
        {
            get => _showDigitSelector;
            set => _showDigitSelector = value;
        }

        public bool TopPressed
        {
            get => (bool)GetValue(TopPressedProperty);
            set
            {
                SetValue(TopPressedProperty, value);
                Pressed = value;
            }
        }

        

        public void Select()
        {
            SelectionEvent?.Invoke(this);
            IsSelected = true;
        }
        
        public void ShowColon()
        {
            //_viewBox_Colon.Visibility = Visibility.Visible;
        }

        private void BottomTouch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                BottomPressed = true;
            }
        }

        private void BottomTouch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Changeable)
            {
                BottomPressed = false;
            }
        }

        private void BottomTouch_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                Select();
                BottomPressed = false;
                if (_currentValue != null)
                {
                    Byte.TryParse(_currentValue.ToString(), out byte b);
                    if (b > 0)
                    {
                        //SetDigit((byte)(b - 1));
                    }
                }
            }
        }

        private void ConfigureEvents()
        {
            _topTouch.MouseDown += TopTouch_MouseDown;
            _topTouch.MouseUp += TopTouch_MouseUp;
            _topTouch.MouseLeave += TopTouch_MouseLeave;
            _bottomTouch.MouseDown += BottomTouch_MouseDown;
            _bottomTouch.MouseUp += BottomTouch_MouseUp;
            _bottomTouch.MouseLeave += BottomTouch_MouseLeave;
            _segmentFourFront.MouseUp += SegmentFourFront_MouseUp;
        }

        private void SegmentFourFront_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Select();
        }

        

        private void TopTouch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                TopPressed = true;
            }
        }

        private void TopTouch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Changeable)
            {
                TopPressed = false;
            }
        }

        private void TopTouch_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                Select();
                TopPressed = false;
                if (_currentValue != null)
                {
                    Byte.TryParse(_currentValue.ToString(), out byte b);
                    if (b < 9)
                    {
                        //SetDigit((byte)(b + 1));
                    }
                }
            }
        }

        


        public override void Decrement()
        {
            throw new NotImplementedException();
        }

        public override void Increment()
        {
            throw new NotImplementedException();
        }
    }
    public class TouchOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return 0.5;
                }
            }
            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}