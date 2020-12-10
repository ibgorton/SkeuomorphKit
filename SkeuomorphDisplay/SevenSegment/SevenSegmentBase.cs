using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using static SkeuomorphCommon.SevenMap;

namespace SkeuomorphDisplay.SevenSegment
{
    /*     SEGMENT NUMBERING
    *
    *          __________
    *        _/   ONE    \_
    *       / \__________/ \
    *      | S |        | T |
    *      | I |        | W |
    *      | X |        | O |
    *      |   |________|   |
    *       \_/  SEVEN   \_/
    *       / \__________/T\
    *      | F |        | H |
    *      | I |        | R |
    *      | V |        | E |
    *      | E |________| E |    __
    *       \_/   FOUR   \_/    /  \ <- DECIMAL POINT
    *         \__________/      \__/
    */

    public abstract class SevenSegmentBase : DisplayControlBase
    {
        private readonly bool[] _bits = new bool[7];

        public SevenSegmentBase() : base()
        {
        }

        public double DecimalDisplayAngle
        {
            get => (double)GetValue(DecimalDisplayAngleProperty);
            set => SetValue(DecimalDisplayAngleProperty, value);
        }

        public double DisplayAngle
        {
            get => (double)GetValue(SegmentDisplayAngleProperty);
            set => SetValue(SegmentDisplayAngleProperty, value);
        }

        protected static readonly DependencyProperty BackgroundFillProperty =
            DependencyProperty.Register(
            name: "BackgroundFill", propertyType: typeof(Brush),
            ownerType: typeof(SevenSegmentBase),
            typeMetadata: new PropertyMetadata(defaultValue: Brushes.Black));

        protected static readonly DependencyProperty BottomPressedProperty =
            DependencyProperty.Register(
            name: "BottomPressed", propertyType: typeof(bool),
            ownerType: typeof(SevenSegmentBase),
            typeMetadata: new PropertyMetadata(defaultValue: false));

        protected static readonly DependencyProperty DecimalDisplayAngleProperty =
            DependencyProperty.Register(
                name: "DecimalDisplayAngle",
                propertyType: typeof(double),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: 8d));

        protected static readonly DependencyProperty SegmentDisplayAngleProperty =
            DependencyProperty.Register(
                name: "DisplayAngle",
                propertyType: typeof(double),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: -8d));

        protected static readonly DependencyProperty ShowDecimalPointProperty =
            DependencyProperty.Register(
                name: "ShowDecimalPoint",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: false));

        protected static readonly DependencyProperty TopPressedProperty =
            DependencyProperty.Register(
                name: "TopPressed",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: false));

        private static readonly DependencyProperty SegmentFiveOnProperty =
            DependencyProperty.Register(
                name: "Segment5On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty SegmentFourOnProperty =
            DependencyProperty.Register(
                name: "Segment4On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty SegmentOneOnProperty =
            DependencyProperty.Register(
                name: "Segment1On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty SegmentSevenOnProperty =
            DependencyProperty.Register(
                name: "Segment7On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty SegmentSixOnProperty =
            DependencyProperty.Register(
                name: "Segment6On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty SegmentThreeOnProperty =
            DependencyProperty.Register(
                name: "Segment3On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        private static readonly DependencyProperty SegmentTwoOnProperty =
            DependencyProperty.Register(
                name: "Segment2On",
                propertyType: typeof(bool),
                ownerType: typeof(SevenSegmentBase),
                typeMetadata: new PropertyMetadata(defaultValue: true));

        public override void SetChar(char c)
        {
            lock (_changeValueLock)
            {
                _bits.GetBitSeven(c: c);
                Segment1On = _bits[0];
                Segment2On = _bits[1];
                Segment3On = _bits[2];
                Segment4On = _bits[3];
                Segment5On = _bits[4];
                Segment6On = _bits[5];
                Segment7On = _bits[6];
            }
        }

        public override void BlankModule()
        {
            lock (_changeValueLock)
            {
                Segment1On =
                Segment2On =
                Segment3On =
                Segment4On =
                Segment5On =
                Segment6On =
                Segment7On = false;
            }
        }

        public bool Segment5On
        {
            get => (bool)GetValue(dp: SegmentFiveOnProperty);
            set => SetValue(dp: SegmentFiveOnProperty, value: value);
        }

        public bool Segment4On
        {
            get => (bool)GetValue(dp: SegmentFourOnProperty);
            set => SetValue(dp: SegmentFourOnProperty, value: value);
        }

        public bool Segment1On
        {
            get => (bool)GetValue(dp: SegmentOneOnProperty); 
            set => SetValue(dp: SegmentOneOnProperty, value: value);
        }

        public bool Segment7On
        {
            get => (bool)GetValue(dp: SegmentSevenOnProperty); 
            set => SetValue(dp: SegmentSevenOnProperty, value: value);
        }

        public bool Segment6On
        {
            get => (bool)GetValue(dp: SegmentSixOnProperty); 
            set => SetValue(dp: SegmentSixOnProperty, value: value);
        }

        public bool Segment3On
        {
            get => (bool)GetValue(dp: SegmentThreeOnProperty); 
            set => SetValue(dp: SegmentThreeOnProperty, value: value);
        }

        public bool Segment2On
        {
            get => (bool)GetValue(dp: SegmentTwoOnProperty); 
            set => SetValue(dp: SegmentTwoOnProperty, value: value);
        }
    }
}