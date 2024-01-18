using System;
using System.ComponentModel;
using System.Windows.Media;

namespace SkeuomorphDisplay.SevenSegment
{
    /// <summary>
    /// Interaction logic for SevenSegmentModule.xaml
    /// </summary>
    public sealed partial class SevenSegmentLED : SevenSegmentBase
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public SevenSegmentLED() : base()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(element: this))
            {
                return;
            }
            ConfigureEvents();
            ShowDigitSelector = false;
            IsSelected = false;
        }

        private int? _currentValue = null;

        //private
        private bool _showDigitSelector;

        public event Action<object> SelectionEvent = delegate { };

        public Brush MaskFill
        {
            get => (Brush)GetValue(dp: MaskFillProperty);
            set => SetValue(dp: MaskFillProperty, value: value);
        }

        public bool BottomPressed
        {
            get => (bool)GetValue(dp: BottomPressedProperty);
            set
            {
                SetValue(dp: BottomPressedProperty, value: value);
                Pressed = value;
            }
        }

        //public
        public int? CurrentValue => _currentValue;

        public double SegmentDisplayAngle
        {
            get => (double)GetValue(dp: SegmentDisplayAngleProperty);
            set => SetValue(dp: SegmentDisplayAngleProperty, value: value);
        }

        public bool ShowDecimalPoint
        {
            get => (bool)GetValue(dp: ShowDecimalPointProperty);
            set => SetValue(dp: ShowDecimalPointProperty, value: value);
        }

        public bool ShowDigitSelector
        {
            get => _showDigitSelector;
            set => _showDigitSelector = value;
        }

        public bool TopPressed
        {
            get => (bool)GetValue(dp: TopPressedProperty);
            set
            {
                SetValue(dp: TopPressedProperty, value: value);
                Pressed = value;
            }
        }

        public void Select()
        {
            SelectionEvent?.Invoke(obj: this);
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
                    if (Byte.TryParse(s: _currentValue.ToString(), result: out byte b))
                    {
                        if (b > 0)
                        {
                            //SetDigit((byte)(b - 1));
                        }
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
            _segmentSeven.MouseUp += SegmentFourFront_MouseUp;
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
                    if (Byte.TryParse(s: _currentValue.ToString(), result: out byte b))
                    {
                        if (b < 9)
                        {
                            //SetDigit((byte)(b + 1));
                        }
                    }
                }
            }
        }

    }
}