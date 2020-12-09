using System;
using System.Collections;
using System.Collections.Specialized;
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
        private BitArray _bits = new(7);

        public SevenSegmentBase() : base() { }
        
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
               "BackgroundFill", typeof(Brush),
               typeof(SevenSegmentBase),
               new UIPropertyMetadata(Brushes.Black));

        protected static readonly DependencyProperty BottomPressedProperty =
                DependencyProperty.Register(
                "BottomPressed", typeof(bool),
                typeof(SevenSegmentBase),
                new UIPropertyMetadata(false));

        protected static readonly DependencyProperty DecimalDisplayAngleProperty =
                DependencyProperty.Register(
                "DecimalDisplayAngle", typeof(double),
                typeof(SevenSegmentBase),
                new UIPropertyMetadata(8d));


        protected static readonly DependencyProperty SegmentDisplayAngleProperty =
                DependencyProperty.Register(
                "DisplayAngle", typeof(double),
                typeof(SevenSegmentBase),
                new UIPropertyMetadata(-8d));

        protected static readonly DependencyProperty ShowDecimalPointProperty =
                DependencyProperty.Register(
                "ShowDecimalPoint", typeof(bool),
                typeof(SevenSegmentBase),
                new UIPropertyMetadata(false));

        protected static readonly DependencyProperty TopPressedProperty =
                DependencyProperty.Register(
                "TopPressed", typeof(bool),
                typeof(SevenSegmentBase),
                new UIPropertyMetadata(false));

        private static readonly DependencyProperty SegmentFiveOnProperty =
            DependencyProperty.Register(
            "Segment5On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true));

        private static readonly DependencyProperty SegmentFourOnProperty =
            DependencyProperty.Register(
            "Segment4On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true));

        private static readonly DependencyProperty SegmentOneOnProperty =
            DependencyProperty.Register(
            "Segment1On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true));

        private static readonly DependencyProperty SegmentSevenOnProperty =
            DependencyProperty.Register(
            "Segment7On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true));

        private static readonly DependencyProperty SegmentSixOnProperty =
            DependencyProperty.Register(
            "Segment6On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true)
            );

        private static readonly DependencyProperty SegmentThreeOnProperty =
            DependencyProperty.Register(
            "Segment3On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true));

        private static readonly DependencyProperty SegmentTwoOnProperty =
            DependencyProperty.Register(
            "Segment2On", 
            typeof(bool),
            typeof(SevenSegmentBase),
            new UIPropertyMetadata(true));

        public override void SetChar(char c)
        {
            lock (_changeValueLock)
            {
                BitArray bits = c.GetBitsSeven();
                Segment1On = bits[0];
                Segment2On = bits[1];
                Segment3On = bits[2];
                Segment4On = bits[3];
                Segment5On = bits[4];
                Segment6On = bits[5];
                Segment7On = bits[6];
            }
        }


        public override void BlankModule()
        {
            lock (_changeValueLock)
            {
                Segment1On = false;
                Segment2On = false;
                Segment3On = false;
                Segment4On = false;
                Segment5On = false;
                Segment6On = false;
                Segment7On = false;
            }
        }

        public override void Increment()
        {
            throw new NotImplementedException();
        }

        public override void Decrement()
        {
            throw new NotImplementedException();
        }

        public bool Segment5On
        {
            get => (bool)GetValue(SegmentFiveOnProperty);
            set => SetValue(SegmentFiveOnProperty, value);
        }

        public bool Segment4On
        {
            get => (bool)GetValue(SegmentFourOnProperty);
            set => SetValue(SegmentFourOnProperty, value);
        }

        public bool Segment1On
        {
            get => (bool)GetValue(SegmentOneOnProperty);
            set => SetValue(SegmentOneOnProperty, value);
        }

        public bool Segment7On
        {
            get => (bool)GetValue(SegmentSevenOnProperty);
            set => SetValue(SegmentSevenOnProperty, value);
        }

        public bool Segment6On
        {
            get => (bool)GetValue(SegmentSixOnProperty);
            set => SetValue(SegmentSixOnProperty, value);
        }

        public bool Segment3On
        {
            get => (bool)GetValue(SegmentThreeOnProperty);
            set => SetValue(SegmentThreeOnProperty, value);
        }

        public bool Segment2On
        {
            get => (bool)GetValue(SegmentTwoOnProperty);
            set => SetValue(SegmentTwoOnProperty, value);
        }
    }
}
