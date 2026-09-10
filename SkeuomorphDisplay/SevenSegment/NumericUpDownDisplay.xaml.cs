using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using SkeuomorphCore;

namespace SkeuomorphDisplay.SevenSegment
{
    /// <summary>
    /// Interaction logic for NumericUpDownDisplay.xaml
    /// </summary>
    public partial class NumericUpDownDisplay : UserControl
    {
        //private
        private double _increment = 0.1;
        private double _value = 0d;

        /// <summary>
        /// Class Constructor
        /// </summary>
        public NumericUpDownDisplay()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(element: this))
                return;

            _value = 0.0;
            _NumericDisplay.ShowSelector = true;
            _NumericDisplay.IncrementChanged += NumericDisplay_IncrementChanged;
        }

        public event Action<double> ValueChanged = delegate { };

        public void SetDecimals(byte value) => _NumericDisplay.SetNumberDecimals(value: value);

        public void SetDigits(byte value) => _NumericDisplay.SetNumberDigits(value: value);

        public double Increment
        {
            get => _increment;
            set
            {
                _increment = value;
                switch (value.ToString())
                {
                    case "0.1":
                        //_NumericDisplay._Module_0.IsSelected = true;
                        break;
                    case "0.01":
                        //_NumericDisplay._Module_01.IsSelected = true;
                        break;
                    case "0.001":
                        //_NumericDisplay._Module_001.IsSelected = true;
                        break;
                }
            }
        }

        public double Maximum
        {
            get => _NumericDisplay.Maximum;
            set => _NumericDisplay.Maximum = value;
        }

        public double Minimum
        {
            get => _NumericDisplay.Minimum;
            set => _NumericDisplay.Minimum = value;
        }

        public void SetValue(double value)
        {
            value = TrimToMaxMin(value: value);
            EnableUpdownButtons(value: value);
            _NumericDisplay.Input = _value = value;
            ValueChanged?.Invoke(obj: value);
        }

        private void NumericDisplay_IncrementChanged(double e) => Increment = e;

        private void Button_IncrementDown_Click(object sender, RoutedEventArgs e)
        {

            if (_NumericDisplay.IntegerCount > 0 && _value > 0)
            {
                //if (_NumericDisplay.SelectedModule?.CurrentValue == 1)
                //    _NumericDisplay.DropDecimalPosition();
            }
            SetValue(value: _value - _increment);
        }

        private void Button_IncrementUp_Click(object sender, RoutedEventArgs e)
        {
            SetValue(value: _value + _increment);
        }

        private void EnableUpdownButtons(double value)
        {
            var pathUp = _PathUp;
            var pathDown = _PathDown;
            if (pathUp is null || pathDown is null)
                return;

            var buttonState = DisplayButtonState.FromValue(value, Minimum, Maximum);
            var disabledBrush = new BrushConverter().ConvertFromString("#073642") as SolidColorBrush ?? Brushes.DarkSlateGray;
            var activeBrush = Brushes.Lime;

            _Button_IncrementUp.IsEnabled = !buttonState.UpDisabled;
            pathUp.Stroke = pathUp.Fill = buttonState.UpDisabled ? disabledBrush : activeBrush;

            _Button_IncrementDown.IsEnabled = !buttonState.DownDisabled;
            pathDown.Stroke = pathDown.Fill = buttonState.DownDisabled ? disabledBrush : activeBrush;
        }
        
        private double TrimToMaxMin(double value)
        {
            value = Math.Min(value, Maximum);
            value = Math.Max(value, Minimum);
            return value;
        }
    }
}
