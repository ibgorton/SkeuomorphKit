namespace SkeuomorphDisplay
{
    public interface IDisplayControl
    {
        double IncrementFactor { get; set; }

        void BlankModule();

        void SetChar(char character);

        void SetColorBrightness();

        enum BrightnessType
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

        enum LedColorType
        {
            Lime,
            Red,
            Blue,
            Orange,
            Yellow,
            Purple
        }

        enum LcdColorType
        {
            Black,
            Inverted
        }
    }
}
