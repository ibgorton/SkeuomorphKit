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
        private char[] _integerChars = new char[0];
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

        public ReadOnlyCollection<char> IntPart => _integerChars.ToList().AsReadOnly();
        
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
            int a = 10 - _integerChars.Length;
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
            
            ulong integerPart = (ulong)value;
            double fractionalPart = Math.Round(value - integerPart, 10);

            _integerChars = integerPart.ToString().ToCharArray().Reverse().ToArray();
            // Fill Digit Values
            for (int i = 0; i < 10; i++)
            {
                int p = 9 - i;
                if (_integerChars.Length > p)
                    _modules[i].SetDigit(_integerChars[p]);
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
                    if ((new char[0]).Count() > p)
                        _modules[i].SetDigit((new char[0])[p]);
                }
            }

            // Blank unused digit locations
            Modules.Take(10 - _integerChars.Length).ToList().ForEach(m => m.BlankModule());
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