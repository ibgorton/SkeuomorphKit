using SkeuomorphCore;
using SkeuomorphDisplay;

namespace SkeuomorphCore.Tests;

public class DisplayValueFormatterTests
{
    private sealed class TestDisplayProfile : IDisplayProfile
    {
        public TestDisplayProfile(string name, int segmentCount, IEnumerable<char> supportedCharacters)
            : this(name, segmentCount, segmentCount, 1, supportedCharacters)
        {
        }

        public TestDisplayProfile(string name, int segmentCount, int width, int height, IEnumerable<char> supportedCharacters)
        {
            Name = name;
            SegmentCount = segmentCount;
            Width = width;
            Height = height;
            SupportedCharacters = [.. supportedCharacters];
        }

        public string Name { get; }

        public int SegmentCount { get; }

        public int Width { get; }

        public int Height { get; }

        private HashSet<char> SupportedCharacters { get; }

        public bool IsSupported(char c)
        {
            return SupportedCharacters.Contains(c) || SupportedCharacters.Contains(char.ToUpperInvariant(c));
        }

        public bool[] GetBits(char c)
        {
            return IsSupported(c) ? new bool[SegmentCount] : new bool[SegmentCount];
        }
    }

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
        var bits = 'A'.GetBits("Rectangle5x7");

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
    public void SixteenMap_GetBitsSixteen_UsesReferenceGlyphPattern()
    {
        var bits = 'A'.GetBits("SixteenSegment");

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
    public void SegmentMaps_MatchDmadsionReferencePatternsForCommonChars()
    {
        var seven = '1'.GetBits("SevenSegment");
        var sixteen = 'A'.GetBits("SixteenSegment");

        Assert.Equal(new[] { false, true, true, false, false, false, false }, seven);
        Assert.Equal(new[] { true, true, true, true, false, false, true, true, false, false, false, true, false, false, false, true }, sixteen);
    }

    [Fact]
    public void DisplayCharacterProfiles_Registry_ProvidesBuiltInLayouts()
    {
        var seven = DisplayCharacterProfiles.Get("SevenSegment");
        var nine = DisplayCharacterProfiles.Get("NineSegment");
        var ten = DisplayCharacterProfiles.Get("TenSegment");
        var fourteen = DisplayCharacterProfiles.Get("FourteenSegment");
        var rectangle = DisplayCharacterProfiles.Get("Rectangle5x7");
        var dotMatrix = DisplayCharacterProfiles.Get("DotMatrix8x8");
        var sixteen = DisplayCharacterProfiles.Get("SixteenSegment");

        Assert.True(seven.IsSupported('A'));
        Assert.True(seven.IsSupported('3'));
        Assert.False(seven.IsSupported('@'));

        Assert.True(nine.IsSupported('A'));
        Assert.True(nine.IsSupported('a'));
        Assert.True(nine.IsSupported('3'));
        Assert.True(nine.IsSupported('$'));

        Assert.True(ten.IsSupported('A'));
        Assert.True(ten.IsSupported('a'));
        Assert.True(ten.IsSupported('3'));
        Assert.True(ten.IsSupported('$'));
        Assert.True(ten.IsSupported('M'));
        Assert.True(ten.IsSupported('@'));

        Assert.True(fourteen.IsSupported('A'));
        Assert.True(fourteen.IsSupported('a'));
        Assert.True(fourteen.IsSupported('3'));
        Assert.True(fourteen.IsSupported('$'));

        Assert.True(rectangle.IsSupported('a'));
        Assert.True(rectangle.IsSupported('?'));
        Assert.False(rectangle.IsSupported('$'));

        Assert.True(dotMatrix.IsSupported('A'));
        Assert.True(dotMatrix.IsSupported('a'));
        Assert.True(dotMatrix.IsSupported('3'));
        Assert.False(dotMatrix.IsSupported('$'));

        Assert.True(sixteen.IsSupported('@'));
        Assert.True(sixteen.IsSupported('z'));
        Assert.True(sixteen.IsSupported('~'));
    }

    [Fact]
    public void DisplayCharacterProfiles_SetCharacterEnabled_ExcludesCharacterFromUseWithoutRemovingMapDefinition()
    {
        Assert.True(DisplayCharacterProfiles.IsSupported("SevenSegment", 'A'));
        Assert.True('A'.GetBits("SevenSegment").Length > 0);

        DisplayCharacterProfiles.SetCharacterEnabled("SevenSegment", 'A', false);
        Assert.False(DisplayCharacterProfiles.IsSupported("SevenSegment", 'A'));
        Assert.True(DisplayCharacterProfiles.Get("SevenSegment").IsSupported('A'));

        DisplayCharacterProfiles.SetCharacterEnabled("SevenSegment", 'A', true);
        Assert.True(DisplayCharacterProfiles.IsSupported("SevenSegment", 'A'));
    }

    [Fact]
    public void TenMap_GetBitsTen_UsesCanonicalTenSegmentOrder()
    {
        var bits = 'A'.GetBits("TenSegment");

        Assert.Equal(10, bits.Length);
        Assert.Equal(new[]
        {
            true, true, true, false,
            true, true, true,
            false, false, false
        }, bits);
    }

    [Fact]
    public void NineMap_GetBitsNine_UsesCanonicalNineSegmentOrder()
    {
        var bits = 'A'.GetBits("NineSegment");

        Assert.Equal(9, bits.Length);
        Assert.Equal(new[]
        {
            true, true, true, false,
            true, true, true,
            false, false
        }, bits);
    }

    [Fact]
    public void TenMap_UsesSevenSegmentGlyphsForSharedCharacters()
    {
        var sevenBits = 'I'.GetBits("SevenSegment");
        var tenBits = 'I'.GetBits("TenSegment");

        Assert.Equal(10, tenBits.Length);
        Assert.Equal(sevenBits[0], tenBits[0]);
        Assert.Equal(sevenBits[1], tenBits[1]);
        Assert.Equal(sevenBits[2], tenBits[2]);
        Assert.Equal(sevenBits[3], tenBits[3]);
        Assert.Equal(sevenBits[4], tenBits[4]);
        Assert.Equal(sevenBits[5], tenBits[5]);
        Assert.Equal(sevenBits[6], tenBits[6]);
        Assert.False(tenBits[7]);
        Assert.False(tenBits[8]);
        Assert.False(tenBits[9]);
    }

    [Fact]
    public void NineMap_UsesSevenSegmentGlyphsForSharedCharacters()
    {
        var sevenBits = 'I'.GetBits("SevenSegment");
        var nineBits = 'I'.GetBits("NineSegment");

        Assert.Equal(9, nineBits.Length);
        Assert.Equal(sevenBits[0], nineBits[0]);
        Assert.Equal(sevenBits[1], nineBits[1]);
        Assert.Equal(sevenBits[2], nineBits[2]);
        Assert.Equal(sevenBits[3], nineBits[3]);
        Assert.Equal(sevenBits[4], nineBits[4]);
        Assert.Equal(sevenBits[5], nineBits[5]);
        Assert.Equal(sevenBits[6], nineBits[6]);
        Assert.False(nineBits[7]);
        Assert.False(nineBits[8]);
    }

    [Fact]
    public void NineMap_UnsupportedCharacters_AreBlank()
    {
        var bits = ';'.GetBits("NineSegment");

        Assert.Equal(9, bits.Length);
        Assert.Equal(new[]
        {
            false, false, false, false,
            false, false, false,
            false, false
        }, bits);
    }

    [Fact]
    public void FourteenMap_GetBitsFourteen_UsesCanonicalHardwareGlyphs()
    {
        var bits = 'A'.GetBits("FourteenSegment");
        var mBits = 'M'.GetBits("FourteenSegment");

        Assert.Equal(14, bits.Length);
        Assert.Equal(new[]
        {
            true, true, true, false,
            true, true, true, true,
            false, true, true,
            false, false, false
        }, bits);

        Assert.Equal(new[]
        {
            false, true, true, false,
            true, true, false, true,
            false, true, false,
            false, false, false
        }, mBits);
    }

    [Fact]
    public void DotMatrix8x8_GetBitsDotMatrix8x8_UsesExpandedGlyphMatrix()
    {
        var bits = 'A'.GetBitsDotMatrix8x8();

        Assert.Equal(64, bits.Length);
        Assert.False(bits[9]);
        Assert.True(bits[10]);
        Assert.True(bits[11]);
        Assert.True(bits[12]);
        Assert.False(bits[13]);
        Assert.False(bits[0]);
        Assert.False(bits[7]);
    }

    [Fact]
    public void DisplayCharacterProfiles_CanRegisterCustomProfiles()
    {
        var custom = new TestDisplayProfile("CustomDisplay", 8, new[] { 'X', 'Y' });

        DisplayCharacterProfiles.Register(custom);

        Assert.True(DisplayCharacterProfiles.IsSupported("CustomDisplay", 'Y'));
        Assert.False(DisplayCharacterProfiles.IsSupported("CustomDisplay", 'Z'));
        Assert.Equal(8, DisplayCharacterProfiles.Get("CustomDisplay").SegmentCount);
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
    public void GlyphLibrary_LowercaseUsesDistinctPatternFromUppercase()
    {
        Assert.True(GlyphLibrary.TryGetPattern('a', out var lowercaseRows));
        Assert.True(GlyphLibrary.TryGetPattern('A', out var uppercaseRows));

        Assert.NotEqual(lowercaseRows, uppercaseRows);
        Assert.Equal("00000", lowercaseRows[0]);
        Assert.Equal("00000", lowercaseRows[1]);
        Assert.Equal("01110", lowercaseRows[2]);
        Assert.Equal("00001", lowercaseRows[3]);
        Assert.Equal("01111", lowercaseRows[4]);
        Assert.Equal("10001", lowercaseRows[5]);
        Assert.Equal("01111", lowercaseRows[6]);
    }

    [Fact]
    public void SevenAndSixteenDisplays_TreatPeriodAsDecimalPointToggle()
    {
        var seven = new SevenSegmentDisplay();
        var sixteen = new SixteenSegmentDisplay();

        seven.SetChar('.');
        sixteen.SetChar('.');

        Assert.True(seven.ShowDecimalPoint);
        Assert.True(sixteen.ShowDecimalPoint);
        Assert.DoesNotContain(true, seven.GetSegments());
        Assert.DoesNotContain(true, sixteen.GetSegments());
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
        var one = '1'.GetBits("SevenSegment");
        var two = '2'.GetBits("SevenSegment");
        var zero = '0'.GetBits("SevenSegment");

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
        var bits = 'P'.GetBits("SevenSegment");

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
        var bits = '@'.GetBits("SevenSegment");

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
