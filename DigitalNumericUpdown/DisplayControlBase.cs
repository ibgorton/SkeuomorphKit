
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalNumericUpdown
{
    public abstract class DisplayControlBase : UserControl
    {
        protected readonly object _changeValueLock = new object();
        public double IncrementFactor { get; set; }
        public DisplayControlBase()
        {
            Loaded += (o, e) =>
            {
                switch (LedColor)
                {
                    case LedColorType.Lime:
                        LedFill = Colors.Lime.CreateLEDBrush((int)Brightness);
                        break;

                    case LedColorType.Red:
                        LedFill = Colors.Red.CreateLEDBrush((int)Brightness);
                        break;

                    case LedColorType.Blue:
                        LedFill = Colors.Blue.CreateLEDBrush((int)Brightness);
                        break;

                    case LedColorType.Orange:
                        LedFill = Colors.Orange.CreateLEDBrush((int)Brightness);
                        break;

                    case LedColorType.Yellow:
                        LedFill = Colors.Yellow.CreateLEDBrush((int)Brightness);
                        break;

                    case LedColorType.Purple:
                        LedFill = Colors.Purple.CreateLEDBrush((int)Brightness);
                        break;
                }
            };
        }

        public abstract void Increment();

        public abstract void Decrement();

        public abstract void BlankModule();

        public abstract void SetChar(char character);

        protected static readonly DependencyProperty PressedProperty =
                DependencyProperty.Register(
                "Pressed", typeof(bool),
                typeof(DisplayControlBase),
                new UIPropertyMetadata(false)
                );



        public static readonly DependencyProperty IsSelectedProperty =
                DependencyProperty.Register(
                "IsSelected", typeof(bool),
                typeof(SevenSegmentBase),
                new UIPropertyMetadata(true)
                );
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        protected bool Pressed
        {
            get => (bool)GetValue(PressedProperty);
            set
            {
                SetValue(PressedProperty, value);
                DisplayScale = value ? 0.495 : 0.5;
            }
        }

        protected double DisplayScale
        {
            get => (double)GetValue(DisplayScaleProperty);
            set => SetValue(DisplayScaleProperty, value);
        }

        protected static readonly DependencyProperty DisplayScaleProperty =
                DependencyProperty.Register(
                "DisplayScale", typeof(double),
                typeof(DisplayControlBase),
                new UIPropertyMetadata(0.5)
                );

        protected enum BrightnessType
        {
            Negative9 = -9,
            Negative8 = -8,
            Negative7 = -7,
            Negative6 = -6,
            Negative5 = -5,
            Negative4 = -4,
            Negative3 = -3,
            Negative2 = -2,
            Negative1 = -1,
            Normal = 0,
            Positive1 = 1,
            Positive2 = 2,
            Positive3 = 3,
            Positive4 = 4,
            Positive5 = 5,
            Positive6 = 6,
            Positive7 = 7,
            Positive8 = 8,
            Positive9 = 9
        }

        public enum LedColorType
        {
            Lime,
            Red,
            Blue,
            Orange,
            Yellow,
            Purple
        }

        private static readonly DependencyProperty LedFillProperty =
                DependencyProperty.Register(
                "LedFill", typeof(Brush),
                typeof(DisplayControlBase),
                new UIPropertyMetadata(Brushes.Lime)
                );

        public Brush LedFill
        {
            get => (Brush)GetValue(LedFillProperty);
            set => SetValue(LedFillProperty, value);
        }

        private static readonly DependencyProperty BrightnessProperty =
                DependencyProperty.Register(
                "Brightness", typeof(BrightnessType),
                typeof(DisplayControlBase),
                new UIPropertyMetadata(BrightnessType.Positive2)
                );

        private BrightnessType Brightness
        {
            get => (BrightnessType)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }


        protected static readonly DependencyProperty LedColorProperty =
                DependencyProperty.Register(
                "LedColor", typeof(LedColorType),
                typeof(DisplayControlBase)
                );

        public LedColorType LedColor
        {
            get => (LedColorType)GetValue(LedColorProperty);
            set => SetValue(LedColorProperty, value);
        }

        private static readonly DependencyProperty ChangeableProperty =
                DependencyProperty.Register(
                "ChangeableProperty", typeof(bool),
                typeof(DisplayControlBase),
                new UIPropertyMetadata(true)
                );

        protected bool Changeable
        {
            get => (bool)GetValue(ChangeableProperty);
            set => SetValue(ChangeableProperty, value);
        }
    }
}
