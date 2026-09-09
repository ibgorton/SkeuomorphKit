using System;

namespace SkeuomorphCommon
{
    public readonly record struct DisplayButtonState(bool UpDisabled, bool DownDisabled)
    {
        public static DisplayButtonState FromValue(double value, double minimum, double maximum)
        {
            var upDisabled = Math.Abs(value - maximum) < double.Epsilon && Math.Abs(value) > double.Epsilon;
            var downDisabled = Math.Abs(value - minimum) < double.Epsilon;
            return new DisplayButtonState(upDisabled, downDisabled);
        }
    }
}
