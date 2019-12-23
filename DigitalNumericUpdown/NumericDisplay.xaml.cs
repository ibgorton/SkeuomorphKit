using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DigitalNumericUpdown
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NumericDisplay : UserControl
    {
        //private
        private readonly BitArray _decimals = new BitArray(new bool[10]);
        private readonly BitArray _digits = new BitArray(new bool[10]);
        private bool _bool_ShowSelector;
        private readonly List<SevenSegmentModule> _modules = new List<SevenSegmentModule>();
        private int _integerCount;
        private double _maximum = 9999999999.9999999999;
        private double _minimum = -999999999.0;

        //public
        public int IntegerCount => _integerCount;
        public SevenSegmentModule SelectedModule => _modules.FirstOrDefault(m => m.IsSelected);

        //events
        public event Action<double> IncrementChanged = delegate { };

        /// <summary>
        /// Class constructor
        /// </summary>
        public NumericDisplay()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _modules = LogicalTreeHelper.GetChildren(_StackPanel).OfType<SevenSegmentModule>().ToList();
            HookupSelectionEvents();
            ShowSelector = false;
        }

        public void SetNumberDecimals(byte value)
        {
            value = Math.Min(value, (byte)10);
            //zero out the array
            for (int i = 0; i < _decimals.Count; i++)
                _decimals[i] = false;
            for (int i = 0; i < value; i++)
                _decimals[i] = true;
            SetDecimalsVisibility();
        }


        public void SetNumberDigits(byte value)
        {
            value = Math.Min(value, (byte)10);
            //zero out the array
            for (int i = 0; i < _digits.Count; i++)
                _digits[i] = false;
            for (int i = 0; i < value; i++)
                _digits[i] = true;
            SetDigitsVisibility();
        }

        public double Input
        {
            set => ProcessInput(value);
        }

        public double Maximum
        {
            get => _maximum;
            set
            {
                value = Math.Min(value, 9999999999.9999999999);
                _maximum = value;
            }
        }

        public double Minimum
        {
            get => _minimum;
            set
            {
                value = Math.Max(value, -999999999.9999999999);
                _minimum = value;
            }
        }

        public bool ShowSelector
        {
            get => _bool_ShowSelector;
            set
            {
                _modules.ForEach(m => m.ShowDigitSelector = value);
                _bool_ShowSelector = value;
            }
        }

        internal void DropDecimalPosition()
        {
            int a = 10 - _integerCount;
            if (_modules[a].IsSelected)
            {
                int b = Math.Min(a + 1, 9);
                _modules[b].Select();
            }
        }

        private void HookupSelectionEvents()
        {
            foreach (SevenSegmentModule m in Modules)
            {
                m.SelectionEvent += SevenSegmentDisplayModule_SelectionEvent;
            };
        }

        public ReadOnlyCollection<SevenSegmentModule> Modules => _modules.AsReadOnly();

        private void ProcessInput(double value)
        {
            value = Math.Min(value, Maximum);
            value = Math.Max(value, Minimum);

            long integerPart = (long)value;
            double fractionalPart = Math.Round(value - integerPart, 10);

            char[] integerChars = integerPart.ToString().ToCharArray().Reverse().ToArray();
            _integerCount = integerChars.Length;
            // Fill Digit Values
            for (int i = 0; i < 10; i++)
            {
                int p = 9 - i;
                if (integerChars.Length > p)
                    _modules[i].SetDigit(integerChars[p]);
            }

            if (fractionalPart > 0d)
            {
                //remove the leading '0.'
                string trimLeading = fractionalPart.ToString().Remove(0, 2);
                char[] fractionChars = trimLeading.ToCharArray();
                // Fill Decimal Values
                for (int i = 10; i < 20; i++)
                {
                    int p = i - 10;
                    if (fractionChars.Length > p)
                        _modules[i].SetDigit(fractionChars[p]);
                }
            }

            // Blank unused digit locations
            Modules.Take(10 - integerChars.Length).ToList().ForEach(m => m.BlankModule());
        }

        private void SetDecimalsVisibility()
        {
            for (int i = 10; i < 20; i++)
            {
                SetNumberModuleVisibility(_modules[i], _decimals[i - 10]);
            }
        }

        private void SetDigitsVisibility()
        {
            for (int i = 0; i < 10; i++)
            {
                SetNumberModuleVisibility(_modules[i], _digits[9 - i]);
            }
        }

        private void SetNumberModuleVisibility(UserControl module, bool state)
        {
            module.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SevenSegmentDisplayModule_SelectionEvent(object sender)
        {
            _modules.ForEach(m => m.IsSelected = false);
            if (sender is SevenSegmentModule s)
            {
                s.IsSelected = true;
                IncrementChanged?.Invoke(s.Increment);
            }
        }
    }
}