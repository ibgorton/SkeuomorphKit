using SkeuomorphCore;
using SkeuomorphDisplay;

namespace SkeuomorphCore.Tests;

public class DisplayValueFormatterTests
{
    private sealed class TestDisplayHost : ISegmentDisplayHost
    {
        public int ApplyCount { get; private set; }
        public int RefreshCount { get; private set; }

        public void ApplySegmentState(int segmentIndex, bool isOn)
        {
            ApplyCount++;
        }

        public void Refresh()
        {
            RefreshCount++;
        }
    }

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
    public void DisplayCharacterProfiles_SevenSegment_RestrictsToReadableSubset()
    {
        Assert.True(DisplayCharacterProfiles.IsSupportedForSevenSegment('A'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForSevenSegment('3'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForSevenSegment('-'));
        Assert.False(DisplayCharacterProfiles.IsSupportedForSevenSegment('@'));
    }

    [Fact]
    public void DisplayCharacterProfiles_Rectangle5x7_SupportsGeneralTextSubset()
    {
        Assert.True(DisplayCharacterProfiles.IsSupportedForRectangle5x7('a'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForRectangle5x7('?'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForRectangle5x7('Z'));
        Assert.False(DisplayCharacterProfiles.IsSupportedForRectangle5x7('$'));
    }

    [Fact]
    public void DisplayCharacterProfiles_SixteenSegment_ProvidesBroadAsciiSupport()
    {
        Assert.True(DisplayCharacterProfiles.IsSupportedForSixteenSegment('@'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForSixteenSegment('z'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForSixteenSegment('%'));
        Assert.True(DisplayCharacterProfiles.IsSupportedForSixteenSegment('~'));
    }

    [Fact]
    public void GlyphLibrary_ExplicitlySupportsCommonPunctuationAndLowercase()
    {
        Assert.True(GlyphLibrary.TryGetPattern('.', out var dotRows));
        Assert.True(GlyphLibrary.TryGetPattern('a', out var lowercaseRows));
        Assert.Equal(7, dotRows.Length);
        Assert.Equal(7, lowercaseRows.Length);
    }

    [Fact]
    public void CharacterMap_CanCreateAndEditGlyphsWithIntegerMask()
    {
        var map = new CharacterMap(5, 7);
        map.Set('A', new[] { "01110", "10001", "10001", "11111", "10001", "10001", "10001" });

        Assert.True(map.TryGetMask('A', out var mask));
        Assert.Equal(0b01110UL, map.GetMask('A') & 0b11111UL);
        Assert.Equal(35, map.GetBits('A').Length);

        map.Set('B', new[] { "11110", "10001", "10001", "11110", "10001", "10001", "11110" });
        Assert.True(map.TryGetMask('B', out _));
        Assert.NotEqual(map.GetMask('A'), map.GetMask('B'));

        var bits = new bool[] { true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        var maskFromBits = CharacterMap.MaskFromBits(bits);
        var roundTripBits = CharacterMap.BitsFromMask(maskFromBits, bits.Length);

        Assert.Equal(bits, roundTripBits);
    }

    [Fact]
    public void CharacterMap_RejectsInvalidRowSizes()
    {
        var map = new CharacterMap(5, 7);

        var ex = Assert.Throws<ArgumentException>(() => map.Set('X', new[] { "01110", "10001" }));
        Assert.Contains("Expected 7 rows", ex.Message);
    }

    [Fact]
    public void SevenMap_GetBitsSeven_UsesWpfSegmentOrdering()
    {
        var one = '1'.GetBitsSeven();
        var two = '2'.GetBitsSeven();
        var zero = '0'.GetBitsSeven();

        Assert.Equal(7, one.Length);
        Assert.False(one[0]);
        Assert.True(one[1]);
        Assert.True(one[2]);
        Assert.False(one[3]);
        Assert.False(one[4]);
        Assert.False(one[5]);
        Assert.False(one[6]);

        Assert.True(two[0]);
        Assert.True(two[1]);
        Assert.False(two[2]);
        Assert.True(two[3]);
        Assert.True(two[4]);
        Assert.False(two[5]);
        Assert.True(two[6]);

        Assert.True(zero[0]);
        Assert.True(zero[1]);
        Assert.True(zero[2]);
        Assert.True(zero[3]);
        Assert.True(zero[4]);
        Assert.True(zero[5]);
        Assert.False(zero[6]);
    }

    [Fact]
    public void SevenMap_GetBitSeven_ExpandsReadableAlphaSubset()
    {
        var bits = 'P'.GetBitsSeven();

        Assert.Equal(7, bits.Length);
        Assert.True(bits[0]);
        Assert.True(bits[1]);
        Assert.True(bits[2]);
        Assert.False(bits[3]);
        Assert.True(bits[4]);
        Assert.True(bits[5]);
        Assert.True(bits[6]);
    }

    [Fact]
    public void SevenMap_GetBitSeven_UnsupportedCharacter_BlanksSegmentArray()
    {
        bool[] bits = new bool[7];

        bits.GetBitSeven('@');

        Assert.All(bits, value => Assert.False(value));
    }

    [Fact]
    public void SegmentDisplayConnection_Bind_IsIdempotentForSameHost()
    {
        var display = new SevenSegmentDisplay();
        var connection = new SegmentDisplayConnection<SevenSegmentDisplay>(display);
        var host = new TestDisplayHost();

        connection.Bind(host);
        connection.Bind(host);
        display.SetSegmentState(0, true);

        Assert.True(connection.IsBound);
        Assert.Equal(1, host.ApplyCount);
        Assert.Equal(1, host.RefreshCount);
    }

    [Fact]
    public void SegmentDisplayConnection_Unbind_StopsFurtherHostCallbacks()
    {
        var display = new SevenSegmentDisplay();
        var connection = new SegmentDisplayConnection<SevenSegmentDisplay>(display);
        var host = new TestDisplayHost();

        connection.Bind(host);
        connection.Unbind();
        display.SetSegmentState(0, true);

        Assert.False(connection.IsBound);
        Assert.Equal(0, host.ApplyCount);
        Assert.Equal(0, host.RefreshCount);
    }
}
