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
        private readonly BitArray _BitArray_decimals = new BitArray(new bool[10]);
        private readonly BitArray _BitArray_digits = new BitArray(new bool[10]);
        private bool _bool_ShowSelector;
        private byte[] _intPart = new byte[0];
        private readonly List<SevenSegmentModule> _modules = new List<SevenSegmentModule>();

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
            _Module_1.EnableDecimalPoint(true);
            ShowSelector = false;
        }

        public void SetDecimals(byte value)
        {
            value = Math.Min(value, (byte)10);
            //zero out the array
            for (int i = 0; i < _BitArray_decimals.Count; i++)
                _BitArray_decimals[i] = false;
            for (int i = 0; i < value; i++)
                _BitArray_decimals[i] = true;
            SetDecimalsVisibility();
        }

        public void SetDigits(byte value)
        {
            value = Math.Min(value, (byte)10);
            //zero out the array
            for (int i = 0; i < _BitArray_digits.Count; i++)
                _BitArray_digits[i] = false;
            for (int i = 0; i < value; i++)
                _BitArray_digits[i] = true;
            SetDigitsVisibility();
        }
        
        public double Input
        {
            set => ProcessInput(value);
        }

        public ReadOnlyCollection<byte> IntPart => _intPart.ToList().AsReadOnly();
        
        public double Maximum { get; set; }

        public double Minimum { get; set; }

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
            int a = 10 - _intPart.Length;
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
            if (ShowSelector)
            {
                value = Math.Min(value, Maximum);
                value = Math.Max(value, Minimum);
            }
            ulong intPart = (ulong)value;
            double fractionalPart = Math.Round(value - intPart, 10);
            _intPart = intPart.ToString().Select(c => (byte)char.GetNumericValue(c)).Reverse().ToArray();
            byte[] byteArray_fracPart = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            
            if (fractionalPart > 0d)
            {
                byte[] fp = fractionalPart.ToString().Remove(0, 2).Select(c => (byte)char.GetNumericValue(c)).ToArray();
                {
                    int count = 0;
                    foreach (byte i in fp)
                    {
                        byteArray_fracPart[count] = i;
                        count++;
                    }
                }
            }
            
            // Blank unused digit locations
            Modules.Take(10 - _intPart.Length).ToList().ForEach(m => m.SetDigit(10));
            // Fill Digit Values
            for (int i = 0; i < 10; i++)
            {
                int p = 9 - i;
                if (_intPart.Length > p)
                    _modules[i].SetDigit(_intPart[p]);
            }
            // Fill Decimal Values
            for (int i = 10; i < 20; i++)
            {
                int p = i - 10;
                if (byteArray_fracPart.Count() > p)
                    _modules[i].SetDigit(byteArray_fracPart[p]);
            }
        }

        private void SetDecimalsVisibility()
        {
            for (int i = 10; i < 20; i++)
            {
                SetNumberModuleVisibility(_modules[i], _BitArray_decimals[i - 10]);
            }
        }

        private void SetDigitsVisibility()
        {
            for (int i = 0; i < 10; i++)
            {
                SetNumberModuleVisibility(_modules[i], _BitArray_digits[9 - i]);
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