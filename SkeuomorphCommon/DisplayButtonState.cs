using System;

namespace SkeuomorphCommon
{
    public readonly record struct DisplayButtonState(bool UpDisabled, bool DownDisabled)
    {
        public static DisplayButtonState FromValue(double value, double minimum, double maximum)
        {
            return new DisplayButtonState(value >= maximum, value <= minimum);
        }
    }
}
