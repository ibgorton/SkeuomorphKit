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
            return SupportedCharacters.Contains(c);
        }

        public bool[] GetBits(char c)
        {
            return IsSupported(c) ? new bool[SegmentCount] : new bool[SegmentCount];
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
    public void ParseDisplayParts_ZeroValue_IsNotNegative()
    {
        var result = DisplayValueFormatter.ParseDisplayParts(-0d);

        Assert.False(result.Negative);
        Assert.Equal(new[] { '0' }, result.IntegerChars);
        Assert.Empty(result.FractionChars);
    }

    [Fact]
    public void DisplayCharacterProfiles_Registry_ProvidesBuiltInLayouts()
    {
        var seven = DisplayCharacterProfiles.Get("SevenSegment");
        var nine = DisplayCharacterProfiles.Get("NineSegmentSlash");
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
        Assert.True(ten.IsSupported('A'));
        Assert.True(fourteen.IsSupported('A'));
        Assert.True(rectangle.IsSupported('a'));
        Assert.True(dotMatrix.IsSupported('A'));
        Assert.True(sixteen.IsSupported('@'));
    }

    [Fact]
    public void DisplayCharacterProfiles_NineSegmentVariantProfile_IsAvailable()
    {
        var profile = DisplayCharacterProfiles.Get("NineSegmentSlash");
        var map = DisplayCharacterProfiles.GetMap("NineSegmentSlash");

        Assert.Equal("NineSegmentSlash", profile.Name);
        Assert.Equal("NineSegmentSlash", map.Name);
        Assert.True(DisplayCharacterProfiles.IsSupported("NineSegmentSlash", 'A'));
    }

    [Fact]
    public void DisplayCharacterProfiles_SetCharacterEnabled_UpdatesProfileStateImmediately()
    {
        Assert.True(DisplayCharacterProfiles.IsSupported("SevenSegment", 'A'));
        Assert.True('A'.GetBits("SevenSegment").Length > 0);

        DisplayCharacterProfiles.SetCharacterEnabled("SevenSegment", 'A', false);
        Assert.False(DisplayCharacterProfiles.IsSupported("SevenSegment", 'A'));
        Assert.False(DisplayCharacterProfiles.Get("SevenSegment").IsSupported('A'));

        DisplayCharacterProfiles.SetCharacterEnabled("SevenSegment", 'A', true);
        Assert.True(DisplayCharacterProfiles.IsSupported("SevenSegment", 'A'));
        Assert.True(DisplayCharacterProfiles.Get("SevenSegment").IsSupported('A'));
    }

    [Fact]
    public void DisplayCharacterProfiles_SpaceCharacter_IsEnabledAndEditable()
    {
        Assert.True(DisplayCharacterProfiles.IsSupported("SevenSegment", ' '));
        DisplayCharacterProfiles.SetCharacterEnabled("SevenSegment", ' ', false);
        Assert.False(DisplayCharacterProfiles.IsSupported("SevenSegment", ' '));
        DisplayCharacterProfiles.SetCharacterEnabled("SevenSegment", ' ', true);
        Assert.True(DisplayCharacterProfiles.IsSupported("SevenSegment", ' '));
    }

    [Fact]
    public void DisplayCharacterProfiles_FourteenSegmentProfile_TracksRuntimeState()
    {
        var profile = DisplayCharacterProfiles.Get("FourteenSegment");

        Assert.True(profile.IsSupported('M'));

        DisplayCharacterProfiles.SetCharacterEnabled("FourteenSegment", 'M', false);
        Assert.False(profile.IsSupported('M'));

        DisplayCharacterProfiles.SetCharacterEnabled("FourteenSegment", 'M', true);
        Assert.True(profile.IsSupported('M'));
    }

    [Fact]
    public void GlyphMapCatalog_DefaultDefinitions_AreJsonSerializables()
    {
        var json = GlyphMapCatalog.BuiltInJson["SevenSegment"];
        var definition = GlyphMapDefinition.FromJson(json, "SevenSegment");

        Assert.Equal("SevenSegment", definition.Name);
        Assert.True(definition.Characters.ContainsKey("A"));
        Assert.True(definition.ToMasks().ContainsKey('A'));
    }

    [Fact]
    public void DisplayCharacterProfiles_RegisterMapJson_AllowsRuntimeConfiguration()
    {
        var json = "{\n  \"Name\": \"CustomSeven\",\n  \"SegmentCount\": 7,\n  \"Characters\": {\n    \"A\": \"0x7E\",\n    \"B\": \"null\"\n  }\n}";

        DisplayCharacterProfiles.RegisterMapJson("CustomSeven", json);

        Assert.True(DisplayCharacterProfiles.IsSupported("CustomSeven", 'A'));
        Assert.False(DisplayCharacterProfiles.IsSupported("CustomSeven", 'B'));
    }

    [Fact]
    public void DotMatrix8x8_UsesBuiltInLayoutAndStyles()
    {
        var bits = DisplayCharacterProfiles.Get("DotMatrix8x8").GetBits('A');
        Assert.Equal(64, bits.Length);
        Assert.True(bits[2]);
        Assert.True(bits[3]);
        Assert.True(bits[4]);

        var classic = DotMatrix8x8Map.GetBits('A', DotMatrix8x8GlyphStyles.Classic);
        Assert.Equal(64, classic.Length);
        Assert.True(classic[2]);
        Assert.True(classic[3]);
        Assert.True(classic[9]);
        Assert.True(classic[10]);
        Assert.True(classic[17]);
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
    public void DisplayCharacterProfiles_CanRegisterCustomProfiles()
    {
        var custom = new TestDisplayProfile("CustomDisplay", 8, new[] { 'X', 'Y' });

        DisplayCharacterProfiles.Register(custom);

        Assert.True(DisplayCharacterProfiles.IsSupported("CustomDisplay", 'Y'));
        Assert.False(DisplayCharacterProfiles.IsSupported("CustomDisplay", 'Z'));
        Assert.Equal(8, DisplayCharacterProfiles.Get("CustomDisplay").SegmentCount);
    }
}
