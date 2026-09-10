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
    public void GetFractionDisplayChars_ForVerySmallFraction_DoesNotUseScientificNotation()
    {
        var result = DisplayValueFormatter.GetFractionDisplayChars(0.0000001234);

        Assert.Equal(new[] { '0', '0', '0', '0', '0', '0', '1', '2', '3', '4' }, result);
    }

    [Fact]
    public void ParseDisplayParts_ZeroValue_IsNotNegative()
    {
        var result = DisplayValueFormatter.ParseDisplayParts(-0d);

        Assert.False(result.Negative);
        Assert.Equal(new[] { '0' }, result.IntegerChars);
        Assert.Empty(result.FractionChars);
    }

    [Fact]
    public void GlyphLibrary_TryGetPattern_UsesSharedCanonicalMatrix()
    {
        var supported = GlyphLibrary.TryGetPattern('A', out var rows);

        Assert.True(supported);
        Assert.Equal(7, rows.Length);
        Assert.Equal("01110", rows[0]);
        Assert.Equal("10001", rows[1]);
        Assert.Equal("11111", rows[3]);
        Assert.Equal("10001", rows[6]);
    }

    [Fact]
    public void RectangleMap_GetBitsRectangle_UsesSevenByFiveGlyphPattern()
    {
        var bits = 'A'.GetBitsRectangle();

        Assert.Equal(35, bits.Length);
        Assert.False(bits[0]);
        Assert.True(bits[1]);
        Assert.True(bits[2]);
        Assert.True(bits[3]);
        Assert.False(bits[4]);
        Assert.True(bits[5]);
        Assert.True(bits[10]);
        Assert.True(bits[16]);
        Assert.True(bits[19]);
    }

    [Fact]
    public void SixteenMap_GetBitsSixteen_UsesSupportedGlyphPattern()
    {
        var bits = 'A'.GetBitsSixteen();

        Assert.Equal(16, bits.Length);
        Assert.True(bits[0]);
        Assert.True(bits[1]);
        Assert.True(bits[2]);
        Assert.True(bits[3]);
        Assert.False(bits[4]);
        Assert.False(bits[5]);
        Assert.True(bits[6]);
        Assert.True(bits[7]);
        Assert.False(bits[8]);
        Assert.False(bits[9]);
        Assert.False(bits[10]);
        Assert.True(bits[11]);
        Assert.False(bits[12]);
        Assert.False(bits[13]);
        Assert.False(bits[14]);
        Assert.True(bits[15]);
    }

    [Fact]
    public void SevenMap_GetBitSeven_UnsupportedCharacter_BlanksSegmentArray()
    {
        bool[] bits = new bool[7];

        bits.GetBitSeven('Z');

        Assert.All(bits, value => Assert.False(value));
    }
}
