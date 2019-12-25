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
    /// <summary>
    /// Interaction logic for SevenSegmentModule.xaml
    /// </summary>
    public partial class SevenSegmentModule : UserControl
    {
        /* Segment Numbering
         * 
         *  |    ONE    |
         * | |         |T|
         * |T|         |H|
         * |W|         |R|
         * |O|         |E|
         * | |         |E|
         *  |    FOUR   |
         * |F|         | | 
         * |I|         |S|
         * |V|         |I|
         * |E|         |X|
         * | |         | |
         *  |   SEVEN   |  ( ) <- DECIMAL POINT
         *  
         */

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

        //private
        private bool _showDigitSelector;
        private int? _currentValue = null;
        private readonly object _changeValueLock = new object();

        //public
        public int? CurrentValue => _currentValue;

        /// <summary>
        /// Class constructor
        /// </summary>
        public SevenSegmentModule()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            ConfigureEvents();
            ShowDigitSelector = false;
            IsSelected = false;
            Loaded += (o, e) =>
            {
                //Clear the display
                SetDigit(10);
                switch (SegmentColor)
                {
                    case SegmentColorType.Lime:
                        SegmentFill = CreateGlowBrush(Colors.Lime);
                        break;
                    case SegmentColorType.Red:
                        SegmentFill = CreateGlowBrush(Colors.Red);
                        break;
                    case SegmentColorType.Blue:
                        SegmentFill = CreateGlowBrush(Colors.Blue);
                        break;
                    case SegmentColorType.Orange:
                        SegmentFill = CreateGlowBrush(Colors.Orange);
                        break;
                    case SegmentColorType.Yellow:
                        SegmentFill = CreateGlowBrush(Colors.Yellow);
                        break;
                    case SegmentColorType.Purple:
                        SegmentFill = CreateGlowBrush(Colors.Purple);
                        break;
                }
            };
        }


        public static readonly DependencyProperty SegmentColourProperty =
        DependencyProperty.Register(
        "SegmentColor", typeof(SegmentColorType),
        typeof(SevenSegmentModule)
        );

        public SegmentColorType SegmentColor
        {
            get => (SegmentColorType)GetValue(SegmentColourProperty);
            set => SetValue(SegmentColourProperty, value);
        }

        public enum SegmentColorType
        {
            Lime,
            Red,
            Blue,
            Orange,
            Yellow,
            Purple
        }

        public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register(
        "Brightness", typeof(BrightnessType),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(BrightnessType.Positive2)
        );

        public BrightnessType Brightness
        {
            get => (BrightnessType)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }

        public enum BrightnessType
        {
            Negative9 = -9,
            Negative8 = -8,
            Negative7 = -7,
            Negative6 = -6,
            Negative5 = -5,
            Negative4 = -4,
            Negative3 = -3,
            Negative2 = -2,
            Negative1 = -1,
            Normal = 0,
            Positive1 = 1,
            Positive2 = 2,
            Positive3 = 3,
            Positive4 = 4,
            Positive5 = 5,
            Positive6 = 6,
            Positive7 = 7,
            Positive8 = 8,
            Positive9 = 9
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
                        SetDigit((byte)(b - 1));
                    }
                }
            }
        }

        private void BottomTouch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                BottomPressed = true;
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
                        SetDigit((byte)(b + 1));
                    }
                }
            }
        }

        private void TopTouch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                TopPressed = true;
            }
        }

        public static readonly DependencyProperty SegmentOneOnProperty =
        DependencyProperty.Register(
        "SegmentOneOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentOneOn
        {
            get => (bool)GetValue(SegmentOneOnProperty);
            set => SetValue(SegmentOneOnProperty, value);
        }

        public static readonly DependencyProperty SegmentTwoOnProperty =
        DependencyProperty.Register(
        "SegmentTwoOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentTwoOn
        {
            get => (bool)GetValue(SegmentTwoOnProperty);
            set => SetValue(SegmentTwoOnProperty, value);
        }

        public static readonly DependencyProperty SegmentThreeOnProperty =
        DependencyProperty.Register(
        "SegmentThreeOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentThreeOn
        {
            get => (bool)GetValue(SegmentThreeOnProperty);
            set => SetValue(SegmentThreeOnProperty, value);
        }

        public static readonly DependencyProperty SegmentFourOnProperty =
        DependencyProperty.Register(
        "SegmentFourOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentFourOn
        {
            get => (bool)GetValue(SegmentFourOnProperty);
            set => SetValue(SegmentFourOnProperty, value);
        }

        public static readonly DependencyProperty SegmentFiveOnProperty =
        DependencyProperty.Register(
        "SegmentFiveOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentFiveOn
        {
            get => (bool)GetValue(SegmentFiveOnProperty);
            set => SetValue(SegmentFiveOnProperty, value);
        }

        public static readonly DependencyProperty SegmentSixOnProperty =
        DependencyProperty.Register(
        "SegmentSixOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentSixOn
        {
            get => (bool)GetValue(SegmentSixOnProperty);
            set => SetValue(SegmentSixOnProperty, value);
        }

        public static readonly DependencyProperty SegmentSevenOnProperty =
        DependencyProperty.Register(
        "SegmentSevenOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );


        public bool SegmentSevenOn
        {
            get => (bool)GetValue(SegmentSevenOnProperty);
            set => SetValue(SegmentSevenOnProperty, value);
        }

        public static readonly DependencyProperty ChangeableProperty =
        DependencyProperty.Register(
        "ChangeableProperty", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool Changeable
        {
            get => (bool)GetValue(ChangeableProperty);
            set => SetValue(ChangeableProperty, value);
        }

        public static readonly DependencyProperty SegmentFillProperty =
        DependencyProperty.Register(
        "SegmentFill", typeof(Brush),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(Brushes.Lime)
        );

        public Brush SegmentFill
        {
            get => (Brush)GetValue(SegmentFillProperty);
            set => SetValue(SegmentFillProperty, value);
        }

        public static readonly DependencyProperty BackgroundFillProperty =
        DependencyProperty.Register(
        "BackgroundFill", typeof(Brush),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(Brushes.Black)
        );

        public Brush BackgroundFill
        {
            get => (Brush)GetValue(BackgroundFillProperty);
            set => SetValue(BackgroundFillProperty, value);
        }


        public event Action<object> SelectionEvent = delegate { };
        
        public double Increment { get; set; } = 1.0;

        public void ShowColon()
        {
            //_viewBox_Colon.Visibility = Visibility.Visible;
        }

        public static readonly DependencyProperty DisplayScaleProperty =
        DependencyProperty.Register(
        "DisplayScale", typeof(double),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(0.5)
        );

        public double DisplayScale
        {
            get => (double)GetValue(DisplayScaleProperty);
            set => SetValue(DisplayScaleProperty, value);
        }

        public static readonly DependencyProperty DecimalDisplayAngleProperty =
        DependencyProperty.Register(
        "DecimalDisplayAngle", typeof(double),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(8d)
        );

        public double DecimalDisplayAngle
        {
            get => (double)GetValue(DecimalDisplayAngleProperty);
            set => SetValue(DecimalDisplayAngleProperty, value);
        }

        public static readonly DependencyProperty SegmentDisplayAngleProperty =
        DependencyProperty.Register(
        "DisplayAngle", typeof(double),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(-8d)
        );

        public double SegmentDisplayAngle
        {
            get => (double)GetValue(SegmentDisplayAngleProperty);
            set => SetValue(SegmentDisplayAngleProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
        "IsSelected", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty ShowDecimalPointProperty =
        DependencyProperty.Register(
        "ShowDecimalPoint", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(false)
        );

        public bool ShowDecimalPoint
        {
            get => (bool)GetValue(ShowDecimalPointProperty);
            set => SetValue(ShowDecimalPointProperty, value);
        }

        public static readonly DependencyProperty PressedProperty =
        DependencyProperty.Register(
        "Pressed", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(false)
        );

        public bool Pressed
        {
            get => (bool)GetValue(PressedProperty);
            set
            {
                SetValue(PressedProperty, value);
                DisplayScale = value ? 0.495 : 0.5;

            }
        }

        public static readonly DependencyProperty TopPressedProperty =
        DependencyProperty.Register(
        "TopPressed", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(false)
        );

        public bool TopPressed
        {
            get => (bool)GetValue(TopPressedProperty);
            set
            {
                SetValue(TopPressedProperty, value);
                Pressed = value;
            }
        }

        public static readonly DependencyProperty BottomPressedProperty =
        DependencyProperty.Register(
        "BottomPressed", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(false)
        );

        public bool BottomPressed
        {
            get => (bool)GetValue(BottomPressedProperty);
            set
            {
                SetValue(BottomPressedProperty, value);
                Pressed = value;
            }
        }

        public bool ShowDigitSelector
        {
            get => _showDigitSelector;
            set => _showDigitSelector = value;//_viewBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public Brush CreateGlowBrush(Color color)
        {
            RadialGradientBrush gradient = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.5, 0.5),
                Center = new Point(0.5, 0.5)
            };

            GradientStop highlight = new GradientStop();
            GradientStop primary = new GradientStop();
            int b = (int)Brightness;
            if (b > 0)
            {
                highlight.Color = ChangeColorBrightness(color, (int)b / 10d);
                highlight.Offset = 0.0;

                primary.Color = color;
                primary.Offset = 1;
                
            }
            else if (b == 0)
            {
                highlight.Color = primary.Color = color;
            }
            else if (b < 0)
            {
                highlight.Color = primary.Color = ChangeColorBrightness(color, (int)b / 10d);
            }
            gradient.GradientStops.Add(highlight);
            gradient.GradientStops.Add(primary);
            return gradient;
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor; 
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
        }

        public void Select()
        {
            SelectionEvent?.Invoke(this);
            IsSelected = true;
        }
        
        /// <summary>
        /// Sets the number for display
        /// </summary>
        private void SetDigit(byte digit)
        {
            lock (_changeValueLock)
            {
                if (digit < 10)
                    _currentValue = digit;
                else
                    _currentValue = null;

                SegmentOneOn= _bits[digit][0];
                SegmentTwoOn = _bits[digit][1];
                SegmentThreeOn = _bits[digit][2];
                SegmentFourOn = _bits[digit][3];
                SegmentFiveOn = _bits[digit][4];
                SegmentSixOn = _bits[digit][5];
                SegmentSevenOn = _bits[digit][6];
            }
        }

        public void BlankModule()
        {
            SetDigit(10);
        }

        public void SetDigit(char? digit)
        {
            if (digit == null)
                SetDigit(10);
            else if (digit == '-')
                SetDigit(11);
            else
                SetDigit((byte)Char.GetNumericValue((char)digit));
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