using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalNumericUpdown
{
    /// <summary>
    /// Interaction logic for NumericUpDownDisplay.xaml
    /// </summary>
    public partial class NumericUpDownDisplay : UserControl
    {
        //private
        private double _increment = 0.1;
        private double _maximum = 9999999999d;
        private double _minimum = 0d;
        private double _value = 0d;

        public NumericUpDownDisplay()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _value = 0.0;
            _NumericDisplay.ShowSelector = true;
            _NumericDisplay.IncrementChanged += _NumericDisplay_IncrementChanged;
        }

        public event Action<double> ValueChanged = delegate { };
        
        public void SetDecimals(byte value)
        {
            _NumericDisplay.SetDecimals(value);
        }

        public void SetDigits(byte value)
        {
            _NumericDisplay.SetDigits(value);
        }
        
        public double Increment
        {
            get => _increment;
            set
            {
                _increment = value;
                switch (value.ToString())
                {
                    case "0.1":
                        _NumericDisplay._Module_0.IsSelected = true;
                        break;
                    case "0.01":
                        _NumericDisplay._Module_01.IsSelected = true;
                        break;
                    case "0.001":
                        _NumericDisplay._Module_001.IsSelected = true;
                        break;
                }
            }
        }

        public double Maximum
        {
            get => _maximum;
            set => _maximum = _NumericDisplay.Maximum = value;
        }

        public double Minimum
        {
            get => _minimum;
            set => _minimum = _NumericDisplay.Minimum = value;
        }

        public void SetValue(double value)
        {
            value = TrimToMaxMin(value);
            EnableUpdownButtons(value);
            _NumericDisplay.Input = _value = value;
            ValueChanged?.Invoke(value);
        }
        
        private void _NumericDisplay_IncrementChanged(double e)
        {
            Increment = e;
        }

        private void Button_IncrementDown_Click(object sender, RoutedEventArgs e)
        {
            if (_NumericDisplay.IntPart.Count > 0)
            {
                if (_NumericDisplay.IntPart[_NumericDisplay.IntPart.Count - 1] == 1)
                    _NumericDisplay.DropDecimalPosition();
            }
            SetValue(_value - _increment);
        }

        private void Button_IncrementUp_Click(object sender, RoutedEventArgs e)
        {
            SetValue(_value + _increment);
        }

        private void EnableUpdownButtons(double value)
        {
            if ((Math.Abs(value - Maximum) < double.Epsilon) && Math.Abs(value) > double.Epsilon)
            {
                _Button_IncrementUp.IsEnabled = false;
                _PathUp.Stroke = _PathUp.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#073642");
            }
            else
            {
                _Button_IncrementUp.IsEnabled = true;
            }

            if (Math.Abs(value - Minimum) < double.Epsilon)
            {
                _Button_IncrementDown.IsEnabled = false;
                _PathDown.Stroke = _PathDown.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#073642");
            }
            else
            {
                _Button_IncrementDown.IsEnabled = true;
            }
        }
        
        private double TrimToMaxMin(double value)
        {
            value = Math.Min(value, Maximum);
            value = Math.Max(value, Minimum);
            return value;
        }

    }
}
