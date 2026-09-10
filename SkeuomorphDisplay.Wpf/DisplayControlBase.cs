using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkeuomorphDisplay.Wpf
{
    public abstract class DisplayControlBase : UserControl, IDisplayControl
    {
        protected readonly object _changeValueLock = new();
        public double IncrementFactor { get; set; }

        public DisplayControlBase()
        {
            Loaded += (o, e) => SetColorBrightness();
        }

        public abstract void BlankModule();

        public abstract void SetChar(char character);

        public void SetColorBrightness()
        {
            LedFill = LedColor switch
            {
                IDisplayControl.LedColorType.Lime => Colors.LimeGreen.CreateLEDBrush(brightness: (int)Brightness),
                IDisplayControl.LedColorType.Red => Colors.Red.CreateLEDBrush(brightness: (int)Brightness),
                IDisplayControl.LedColorType.Blue => Colors.RoyalBlue.CreateLEDBrush(brightness: (int)Brightness),
                IDisplayControl.LedColorType.Orange => Colors.DarkOrange.CreateLEDBrush(brightness: (int)Brightness),
                IDisplayControl.LedColorType.Yellow => Colors.Yellow.CreateLEDBrush(brightness: (int)Brightness),
                IDisplayControl.LedColorType.Purple => Colors.Purple.CreateLEDBrush(brightness: (int)Brightness),
                _ => Colors.Lime.CreateLEDBrush(brightness: (int)Brightness),
            };
        }

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
                propertyType: typeof(IDisplayControl.BrightnessType),
                ownerType: typeof(DisplayControlBase),
                typeMetadata: new PropertyMetadata(defaultValue: IDisplayControl.BrightnessType.Positive2));

        private IDisplayControl.BrightnessType Brightness
        {
            get => (IDisplayControl.BrightnessType)GetValue(dp: BrightnessProperty);
            set => SetValue(dp: BrightnessProperty, value: value);
        }

        protected static readonly DependencyProperty LedColorProperty =
            DependencyProperty.Register(
                name: "LedColor", propertyType: typeof(IDisplayControl.LedColorType),
                ownerType: typeof(DisplayControlBase),
                typeMetadata: new PropertyMetadata(defaultValue: IDisplayControl.LedColorType.Lime));

        public IDisplayControl.LedColorType LedColor
        {
            get => (IDisplayControl.LedColorType)GetValue(dp: LedColorProperty);
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
