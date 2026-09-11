using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SkeuomorphCore;

namespace SkeuomorphDisplay.Wpf.SevenSegment;

/// <summary>
/// Displays an editable numeric value using segmented LED-style modules.
/// </summary>
public partial class NumericDisplay : UserControl
{
    private const int DigitCapacity = 10;
    private const int DecimalCapacity = 10;

    //private
    private readonly BitArray _decimals = new(values: new bool[DecimalCapacity]);
    private readonly BitArray _digits = new(values: new bool[DigitCapacity]);
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
    /// Initializes the numeric display control.
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
        value = Math.Min(value, (byte)DecimalCapacity);
        for (int i = 0; i < _decimals.Count; i++)
            _decimals[i] = false;
        for (int i = 0; i < value; i++)
            _decimals[i] = true;
        SetDecimalsVisibility();
    }

    public void SetNumberDigits(byte value)
    {
        value = Math.Min(value, (byte)DigitCapacity);
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
            //_modules.ForEach(m => m.ShowDigitSelector = value);
            _bool_ShowSelector = value;
        }
    }

    internal void DropDecimalPosition()
    {
        int a = DigitCapacity - _integerCount;
        if (_modules[a].IsSelected)
        {
            int b = Math.Min(a + 1, DigitCapacity - 1);
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

        int integerStart = DigitCapacity - displayIntegerChars.Length;
        SetModuleChars(integerStart, displayIntegerChars);

        int decimalStart = DigitCapacity;
        SetModuleChars(decimalStart, fractionChars, DecimalCapacity);
    }

    private void SetModuleChars(int startIndex, char[] chars, int maxCount = DigitCapacity)
    {
        int bounds = Math.Min(chars.Length, maxCount);
        for (int i = 0; i < bounds; i++)
        {
            int moduleIndex = startIndex + i;
            if (moduleIndex >= 0 && moduleIndex < _modules.Count)
            {
                _modules[moduleIndex].SetChar(chars[i]);
            }
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
        for (int i = DigitCapacity; i < DigitCapacity + DecimalCapacity; i++)
        {
            SetNumberModuleVisibility(module: _modules[i], state: _decimals[i - DigitCapacity]);
        }
    }

    private void SetDigitsVisibility()
    {
        for (int i = 0; i < DigitCapacity; i++)
        {
            SetNumberModuleVisibility(module: _modules[i], state: _digits[DigitCapacity - 1 - i]);
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