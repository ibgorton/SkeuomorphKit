using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SkeuomorphCommon;

namespace SkeuomorphDisplay.SevenSegment
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NumericDisplay : UserControl
    {
        //private
        private readonly BitArray _decimals = new(values: new bool[10]);
        private readonly BitArray _digits = new(values: new bool[10]);
        private bool _bool_ShowSelector;
        private readonly List<SevenSegmentBase> _modules = new();
        private int _integerCount;
        private double _maximum = 9999999999.9999999999;
        private double _minimum = -999999999.0;

        //public
        public int IntegerCount => _integerCount;
        public SevenSegmentBase? SelectedModule => _modules.FirstOrDefault(predicate: m => m.IsSelected);

        //events
        public event Action<double> IncrementChanged = delegate { };

        /// <summary>
        /// Class constructor
        /// </summary>
        public NumericDisplay()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(element: this))
                return;

            _modules = LogicalTreeHelper.GetChildren(current: _StackPanel).OfType<SevenSegmentBase>().ToList();
            HookupSelectionEvents();
            ShowSelector = false;
        }

        public void SetNumberDecimals(byte value)
        {
            value = Math.Min(value, (byte)10);
            //zero out the array
            for (int i = 0; i < _decimals.Count; i++)
                _decimals[index: i] = false;
            for (int i = 0; i < value; i++)
                _decimals[index: i] = true;
            SetDecimalsVisibility();
        }


        public void SetNumberDigits(byte value)
        {
            value = Math.Min(value, (byte)10);
            //zero out the array
            for (int i = 0; i < _digits.Count; i++)
                _digits[index: i] = false;
            for (int i = 0; i < value; i++)
                _digits[index: i] = true;
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
                //_modules.ForEach(m => m.ShowDigitSelector = value);
                _bool_ShowSelector = value;
            }
        }

        internal void DropDecimalPosition()
        {
            int a = 10 - _integerCount;
            if (_modules[index: a].IsSelected)
            {
                int b = Math.Min(a + 1, 9);
                //_modules[b].Select();
            }
        }

        private void HookupSelectionEvents()
        {
            foreach (SevenSegmentLED m in Modules)
            {
                m.SelectionEvent += SevenSegmentDisplayModule_SelectionEvent;
            };
        }

        public ReadOnlyCollection<SevenSegmentBase> Modules => _modules.AsReadOnly();

        private void ProcessInput(double value)
        {
            value = Math.Min(value, Maximum);
            value = Math.Max(value, Minimum);

            var (integerChars, fractionChars, negative) = DisplayValueFormatter.ParseDisplayParts(value);
            var displayIntegerChars = negative ? new[] { '-' }.Concat(integerChars).ToArray() : integerChars;

            _integerCount = displayIntegerChars.Length - (negative ? 1 : 0);
            ClearModules();

            // Fill integer digits from the right-most available slot, leaving room for a leading sign.
            int integerStart = 10 - displayIntegerChars.Length;
            for (int i = 0; i < displayIntegerChars.Length; i++)
            {
                int moduleIndex = integerStart + i;
                if (moduleIndex >= 0 && moduleIndex < 10)
                    _modules[moduleIndex].SetChar(displayIntegerChars[i]);
            }

            int decimalStart = 10;
            for (int i = 0; i < fractionChars.Length && i < 10; i++)
            {
                _modules[decimalStart + i].SetChar(fractionChars[i]);
            }
        }

        private void ClearModules()
        {
            foreach (var module in _modules)
            {
                module.BlankModule();
            }
        }

        private void SetDecimalsVisibility()
        {
            for (int i = 10; i < 20; i++)
            {
                SetNumberModuleVisibility(module: _modules[index: i], state: _decimals[index: i - 10]);
            }
        }

        private void SetDigitsVisibility()
        {
            for (int i = 0; i < 10; i++)
            {
                SetNumberModuleVisibility(module: _modules[index: i], state: _digits[index: 9 - i]);
            }
        }

        private void SetNumberModuleVisibility(UserControl module, bool state)
        {
            module.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SevenSegmentDisplayModule_SelectionEvent(object sender)
        {
            _modules.ForEach(action: m => m.IsSelected = false);
            if (sender is SevenSegmentLED s)
            {
                s.IsSelected = true;
                IncrementChanged?.Invoke(obj: s.IncrementFactor);
            }
        }
    }
}