using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkeuomorphDisplay
{
    public abstract class DisplayControlBase : UserControl
    {
        protected readonly object _changeValueLock = new();
        public double IncrementFactor { get; set; }

        public DisplayControlBase()
        {
            Loaded += (o, e) => SetColorBrightness();
        }

        public void SetColorBrightness()
        {
            LedFill = LedColor switch
            {
                LedColorType.Lime => Colors.Lime.CreateLEDBrush(brightness: (int)Brightness),
                LedColorType.Red => Colors.Red.CreateLEDBrush(brightness: (int)Brightness),
                LedColorType.Blue => Colors.Blue.CreateLEDBrush(brightness: (int)Brightness),
                LedColorType.Orange => Colors.Orange.CreateLEDBrush(brightness: (int)Brightness),
                LedColorType.Yellow => Colors.Yellow.CreateLEDBrush(brightness: (int)Brightness),
                LedColorType.Purple => Colors.Purple.CreateLEDBrush(brightness: (int)Brightness),
                _ => Colors.Lime.CreateLEDBrush(brightness: (int)Brightness),
            };
        }

        public abstract void BlankModule();

        public abstract void SetChar(char character);

        protected static readonly DependencyProperty PressedProperty =
            DependencyProperty.Register(
                name: "Pressed", propertyType: typeof(bool),
                ownerType: typeof(DisplayControlBase),
                typeMetadata: new PropertyMetadata(defaultValue: false));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                name: "IsSelected", 
                propertyType: typeof(bool), 
                ownerType: typeof(DisplayControlBase), 
                typeMetadata: new PropertyMetadata(defaultValue: true));

        public bool IsSelected
        {
            get => (bool)GetValue(dp: IsSelectedProperty);
            set => SetValue(dp: IsSelectedProperty, value: value);
        }

        protected bool Pressed
        {
            get => (bool)GetValue(dp: PressedProperty);
            set
            {
                SetValue(dp: PressedProperty, value: value);
                DisplayScale = value ? 0.99 : 1.0;
            }
        }

        protected double DisplayScale
        {
            get => (double)GetValue(dp: DisplayScaleProperty);
            set => SetValue(dp: DisplayScaleProperty, value: value);
        }

        protected static readonly DependencyProperty DisplayScaleProperty =
            DependencyProperty.Register(
                name: "DisplayScale", propertyType: typeof(double),
                ownerType: typeof(DisplayControlBase),
                typeMetadata: new PropertyMetadata(defaultValue: 1.0));

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

        public enum LcdColorType 
        { 
            Black,
            Inverted
        }


        private static readonly DependencyProperty LedFillProperty =
            DependencyProperty.Register(
                name: "LedFill", propertyType: typeof(Brush),
                ownerType: typeof(DisplayControlBase),
                typeMetadata: new PropertyMetadata(defaultValue: Brushes.Lime));

        public Brush LedFill
        {
            get => (Brush)GetValue(dp: LedFillProperty);
            set => SetValue(dp: LedFillProperty, value: value);
        }

        private static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register(
                name: "Brightness", 
                propertyType: typeof(BrightnessType), 
                ownerType: typeof(DisplayControlBase), 
                typeMetadata: new PropertyMetadata(defaultValue: BrightnessType.Positive2));

        private BrightnessType Brightness
        {
            get => (BrightnessType)GetValue(dp: BrightnessProperty);
            set => SetValue(dp: BrightnessProperty, value: value);
        }

        protected static readonly DependencyProperty LedColorProperty =
            DependencyProperty.Register(
                name: "LedColor", propertyType: typeof(LedColorType),
                ownerType: typeof(DisplayControlBase),
                typeMetadata: new PropertyMetadata(defaultValue: LedColorType.Lime));

        public LedColorType LedColor
        {
            get => (LedColorType)GetValue(dp: LedColorProperty);
            set => SetValue(dp: LedColorProperty, value: value);
        }

        public static readonly DependencyProperty ChangeableProperty =
            DependencyProperty.Register(
                name: "ChangeableProperty", 
                propertyType: typeof(bool), 
                ownerType: typeof(DisplayControlBase), 
                typeMetadata: new PropertyMetadata(defaultValue: true));

        public bool Changeable
        {
            get => (bool)GetValue(dp: ChangeableProperty);
            set => SetValue(dp: ChangeableProperty, value: value);
        }
    }
}
