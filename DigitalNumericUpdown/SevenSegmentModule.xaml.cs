using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalNumericUpdown
{
    /// <summary>
    /// Interaction logic for SevenSegmentModule.xaml
    /// </summary>
    public partial class SevenSegmentModule : UserControl
    {
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
                new BitArray(new bool[] { false, false, false, false, false, false, false })
        };

        //private
        private bool _selected;
        private bool _showDigitSelector;

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
            EnableDecimalPoint(false);

            ShowDigitSelector = false;
            _Path_SelectedArrow.Fill = Brushes.Transparent;
            IsSelected = false;
            _viewBox_Colon.Visibility = Visibility.Collapsed;
            Loaded += (o, e) =>
            {
                //Clear the display
                SetDigit(10);
            };
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
                _Path_SelectedArrow.Fill = value ? Brushes.Lime : Brushes.Transparent;
            }
        }
               
        public bool ShowDigitSelector
        {
            get => _showDigitSelector;
            set
            {
                _showDigitSelector = value;
                _viewBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
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
        public void SetDigit(byte digit)
        {
            _segment1.Opacity = _bits[digit][0] ? 1.0 : 0.075;
            _segment2.Opacity = _bits[digit][1] ? 1.0 : 0.075;
            _segment3.Opacity = _bits[digit][2] ? 1.0 : 0.075;
            _segment4.Opacity = _bits[digit][3] ? 1.0 : 0.075;
            _segment5.Opacity = _bits[digit][4] ? 1.0 : 0.075;
            _segment6.Opacity = _bits[digit][5] ? 1.0 : 0.075;
            _segment7.Opacity = _bits[digit][6] ? 1.0 : 0.075;
        }

        /// <summary>
        /// Handles the mousedown (selection) of this digit
        /// Highlights the module while the mouse is down
        /// </summary>
        private void ViewBox_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!ShowDigitSelector)
                return;
            _canvas.Background = Brushes.DarkGreen;
            SelectionEvent?.Invoke(this);
        }

        private void Viewbox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _canvas.Background = Brushes.Black;
        }

        private void Viewbox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _canvas.Background = Brushes.Black;
        }
    }
}