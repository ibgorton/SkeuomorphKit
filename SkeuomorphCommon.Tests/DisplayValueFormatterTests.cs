using SkeuomorphCommon;

namespace SkeuomorphCommon.Tests;

public class DisplayValueFormatterTests
{
    [Fact]
    public void GetIntegerDisplayChars_PositiveValue_UsesDigitsOnly()
    {
        var result = DisplayValueFormatter.GetIntegerDisplayChars(123.45);

        Assert.Equal(new[] { '1', '2', '3' }, result);
    }

    [Fact]
    public void GetIntegerDisplayChars_NegativeValue_IncludesLeadingMinus()
    {
        var result = DisplayValueFormatter.GetIntegerDisplayChars(-123.45);

        Assert.Equal(new[] { '-', '1', '2', '3' }, result);
    }

    [Fact]
    public void GetFractionDisplayChars_FormatsFractionPartWithoutLeadingZeroDot()
    {
        var result = DisplayValueFormatter.GetFractionDisplayChars(12.345);

        Assert.Equal(new[] { '3', '4', '5' }, result);
    }

    [Fact]
    public void SevenMap_GetBitSeven_UnsupportedCharacter_BlanksSegmentArray()
    {
        bool[] bits = new bool[7];

        bits.GetBitSeven('Z');

        Assert.All(bits, value => Assert.False(value));
    }
}
