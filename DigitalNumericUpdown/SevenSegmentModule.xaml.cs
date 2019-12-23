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
         *  |  ONE  |
         * | |     |T|
         * |T|     |H|
         * |W|     |R|
         * |O|     |E|
         * | |     |E|
         *  | FOUR  |
         * |F|     | | 
         * |I|     |S|
         * |V|     |I|
         * |E|     |X|
         * | |     | |
         *  | SEVEN |
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
        private bool _selected;
        private bool _showDigitSelector;
        private int? _currentValue = null;
        private readonly object _changeValueLock = new object();
        private readonly RadialGradientBrush _limeBrush;
        private readonly RadialGradientBrush _redBrush;
        private readonly RadialGradientBrush _blueBrush;
        private readonly RadialGradientBrush _orangeBrush;
        private readonly RadialGradientBrush _yellowBrush;
        private readonly RadialGradientBrush _purpleBrush;
        private readonly RadialGradientBrush _placeHolderBrush = new RadialGradientBrush(Color.FromRgb(0, 255, 0), Color.FromRgb(86, 249, 86));

        //public
        public int? CurrentValue => _currentValue;

        /// <summary>
        /// Class constructor
        /// </summary>
        public SevenSegmentModule()
        {
            InitializeComponent();
            //find brush resources
            _limeBrush = (RadialGradientBrush)TryFindResource("LimeGlow") ?? _placeHolderBrush;
            _redBrush = (RadialGradientBrush)TryFindResource("RedGlow") ?? _placeHolderBrush;
            _blueBrush = (RadialGradientBrush)TryFindResource("BlueGlow") ?? _placeHolderBrush;
            _orangeBrush = (RadialGradientBrush)TryFindResource("OrangeGlow") ?? _placeHolderBrush;
            _yellowBrush = (RadialGradientBrush)TryFindResource("YellowGlow") ?? _placeHolderBrush;
            _purpleBrush = (RadialGradientBrush)TryFindResource("PurpleGlow") ?? _placeHolderBrush;
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
            ConfigureEvents();
            EnableDecimalPoint(false);

            
            
            ShowDigitSelector = false;
            //_Path_SelectedArrow.Fill = Brushes.Transparent;
            IsSelected = false;
            _viewBox_Colon.Visibility = Visibility.Collapsed;
            Loaded += (o, e) =>
            {
                //Clear the display
                SetDigit(10);
                switch (SegmentColour)
                {
                    case SegmentColor.Lime:
                        SegmentFill = _limeBrush;
                        break;
                    case SegmentColor.Red:
                        SegmentFill = _redBrush;
                        break;
                    case SegmentColor.Blue:
                        SegmentFill = _blueBrush;
                        break;
                    case SegmentColor.Orange:
                        SegmentFill = _orangeBrush;
                        break;
                    case SegmentColor.Yellow:
                        SegmentFill = _yellowBrush;
                        break;
                    case SegmentColor.Purple:
                        SegmentFill = _purpleBrush;
                        break;
                }
            };
        }


        public static readonly DependencyProperty SegmentColourProperty =
        DependencyProperty.Register(
        "SegmentColour", typeof(SegmentColor),
        typeof(SevenSegmentModule)
        );

        public SegmentColor SegmentColour
        {
            get { return (SegmentColor)GetValue(SegmentColourProperty); }
            set { SetValue(SegmentColourProperty, value); }
        }

        public enum SegmentColor
        {
            Lime,
            Red,
            Blue,
            Orange,
            Yellow,
            Purple
        }
        
        private void ConfigureEvents()
        {
            _topTouch.MouseDown += TopTouch_MouseDown;
            _topTouch.MouseUp += TopTouch_MouseUp;
            _topTouch.MouseLeave += TopTouch_MouseLeave;
            _bottomTouch.MouseDown += BottomTouch_MouseDown;
            _bottomTouch.MouseUp += BottomTouch_MouseUp;
            _bottomTouch.MouseLeave += BottomTouch_MouseLeave;
        }

        private void BottomTouch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Changeable)
            {
                _grid.Opacity = 0.95;
                _viewBox_eight.Width = 78;
                _segmentOneMask.Width = 156;
                _viewBox_eight.Margin = new Thickness(0, 10, 0, 10);
                _Ellipse_DecimalPlace.Height = 24;
                _Ellipse_DecimalPlace.Width = 24;
                _Ellipse_DecimalPlace.Margin = new Thickness(135, 200, 0, 0);
                BottomTouchFill = Brushes.Transparent;
            }
        }

        private void BottomTouch_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                _grid.Opacity = 0.95;
                _viewBox_eight.Width = 78;
                _segmentOneMask.Width = 156;
                _viewBox_eight.Margin = new Thickness(0, 10, 0, 10);
                _Ellipse_DecimalPlace.Height = 24;
                _Ellipse_DecimalPlace.Width = 24;
                _Ellipse_DecimalPlace.Margin = new Thickness(135, 200, 0, 0);
                BottomTouchFill = Brushes.Transparent;
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
                _grid.Opacity = 0.94;
                _viewBox_eight.Width = 77; 
                _segmentOneMask.Width = 155;
                _viewBox_eight.Margin = new Thickness(0, 11, 0, 11);
                _Ellipse_DecimalPlace.Height = 23.5;
                _Ellipse_DecimalPlace.Width = 23.5;
                _Ellipse_DecimalPlace.Margin = new Thickness(136, 200, 0, 0);
                BottomTouchFill = SegmentFill;
            }
        }

        private void TopTouch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Changeable)
            {
                _grid.Opacity = 0.95;
                _viewBox_eight.Width = 78;

                _segmentOneMask.Width = 156;
                _viewBox_eight.Margin = new Thickness(0, 10, 0, 10);
                _Ellipse_DecimalPlace.Height = 24;
                _Ellipse_DecimalPlace.Width = 24;
                _Ellipse_DecimalPlace.Margin = new Thickness(135, 200, 0, 0);
                TopTouchFill = Brushes.Transparent;
            }
        }

        private void TopTouch_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Changeable)
            {
                _grid.Opacity = 0.95;
                _viewBox_eight.Width = 78;
                _segmentOneMask.Width = 156;
                _viewBox_eight.Margin = new Thickness(0, 10, 0, 10);
                _Ellipse_DecimalPlace.Height = 24;
                _Ellipse_DecimalPlace.Width = 24;
                _Ellipse_DecimalPlace.Margin = new Thickness(135, 200, 0, 0);
                TopTouchFill = Brushes.Transparent;
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
                _grid.Opacity = 0.94;
                _viewBox_eight.Width = 77;
                _segmentOneMask.Width = 155;
                _viewBox_eight.Margin = new Thickness(0, 11, 0,11);
                _Ellipse_DecimalPlace.Height = 23.5;
                _Ellipse_DecimalPlace.Width = 23.5;
                _Ellipse_DecimalPlace.Margin = new Thickness(136, 200, 0, 0);
                TopTouchFill = SegmentFill;
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
            get { return (bool)GetValue(SegmentOneOnProperty); }
            set { SetValue(SegmentOneOnProperty, value); }
        }

        public static readonly DependencyProperty SegmentTwoOnProperty =
        DependencyProperty.Register(
        "SegmentTwoOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentTwoOn
        {
            get { return (bool)GetValue(SegmentTwoOnProperty); }
            set { SetValue(SegmentTwoOnProperty, value); }
        }

        public static readonly DependencyProperty SegmentThreeOnProperty =
        DependencyProperty.Register(
        "SegmentThreeOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentThreeOn
        {
            get { return (bool)GetValue(SegmentThreeOnProperty); }
            set { SetValue(SegmentThreeOnProperty, value); }
        }

        public static readonly DependencyProperty SegmentFourOnProperty =
        DependencyProperty.Register(
        "SegmentFourOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentFourOn
        {
            get { return (bool)GetValue(SegmentFourOnProperty); }
            set { SetValue(SegmentFourOnProperty, value); }
        }

        public static readonly DependencyProperty SegmentFiveOnProperty =
        DependencyProperty.Register(
        "SegmentFiveOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentFiveOn
        {
            get { return (bool)GetValue(SegmentFiveOnProperty); }
            set { SetValue(SegmentFiveOnProperty, value); }
        }

        public static readonly DependencyProperty SegmentSixOnProperty =
        DependencyProperty.Register(
        "SegmentSixOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentSixOn
        {
            get { return (bool)GetValue(SegmentSixOnProperty); }
            set { SetValue(SegmentSixOnProperty, value); }
        }

        public static readonly DependencyProperty SegmentSevenOnProperty =
        DependencyProperty.Register(
        "SegmentSevenOn", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool SegmentSevenOn
        {
            get { return (bool)GetValue(SegmentSevenOnProperty); }
            set { SetValue(SegmentSevenOnProperty, value); }
        }

        public static readonly DependencyProperty ChangeableProperty =
        DependencyProperty.Register(
        "ChangeableProperty", typeof(bool),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(true)
        );

        public bool Changeable
        {
            get { return (bool)GetValue(ChangeableProperty); }
            set { SetValue(ChangeableProperty, value); }
        }

        public static readonly DependencyProperty SegmentFillProperty =
        DependencyProperty.Register(
        "SegmentFill", typeof(Brush),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(Brushes.Lime)
        );

        public Brush SegmentFill
        {
            get { return (Brush)GetValue(SegmentFillProperty); }
            set { SetValue(SegmentFillProperty, value); }
        }

        public static readonly DependencyProperty BackgroundFillProperty =
        DependencyProperty.Register(
        "BackgroundFill", typeof(Brush),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(Brushes.Black)
        );

        public Brush BackgroundFill
        {
            get { return (Brush)GetValue(BackgroundFillProperty); }
            set { SetValue(BackgroundFillProperty, value); }
        }

        public static readonly DependencyProperty TopTouchFillProperty =
        DependencyProperty.Register(
        "TopTouchFill", typeof(Brush),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(Brushes.Transparent)
        );

        public Brush TopTouchFill
        {
            get { return (Brush)GetValue(TopTouchFillProperty); }
            set { SetValue(TopTouchFillProperty, value); }
        }

        public static readonly DependencyProperty BottomTouchFillProperty =
        DependencyProperty.Register(
        "BottomTouchFill", typeof(Brush),
        typeof(SevenSegmentModule),
        new UIPropertyMetadata(Brushes.Transparent)
        );

        public Brush BottomTouchFill
        {
            get { return (Brush)GetValue(BottomTouchFillProperty); }
            set { SetValue(BottomTouchFillProperty, value); }
        }

        public event Action<object> SelectionEvent = delegate { };
        
        public double Increment { get; set; } = 1.0;

        public void ShowColon()
        {
            _viewBox_Colon.Visibility = Visibility.Visible;
        }

        public bool IsSelected
        {
            get => _selected;
            set
            {
                _selected = value;
                //_Path_SelectedArrow.Fill = value ? Brushes.DarkSlateGray : Brushes.Transparent;
            }
        }
               
        public bool ShowDigitSelector
        {
            get => _showDigitSelector;
            set
            {
                _showDigitSelector = value;
                //_viewBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void Select()
        {
            SelectionEvent?.Invoke(this);
        }

        /// <summary>
        /// Enables the decimal point indicator
        /// </summary>
        public void EnableDecimalPoint(bool state)
        {
            _Ellipse_DecimalPlace.Opacity = state ? 1.0 : 0.075;
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
    public class OpacityConverter : IValueConverter
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