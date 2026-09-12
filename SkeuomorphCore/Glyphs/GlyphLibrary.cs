using System;
using System.Collections.Generic;

namespace SkeuomorphCore;

public interface IGlyphLayout
{
    int SegmentCount { get; }
    bool[] GetBits(char c);
}

public static class GlyphLibrary
{
    public const int Width = 5;
    public const int Height = 7;

    public static CharacterMap CreateRectangleMap()
    {
        var map = new CharacterMap(Width, Height);
        foreach (var pair in Patterns)
        {
            map.Set(pair.Key, pair.Value);
        }

        return map;
    }

    private static readonly Dictionary<char, ulong> Patterns = new()
    {
        [' '] = 0x00000000000UL,
        ['!'] = 0x00100421084UL,
        ['"'] = 0x000000000C6UL,
        ['#'] = 0x00000AFABEAUL,
        ['%'] = 0x000019D1173UL,
        ['&'] = 0x0059312AA2EUL,
        ['\''] = 0x00000000084UL,
        ['('] = 0x00208210888UL,
        [')'] = 0x00088842082UL,
        ['+'] = 0x000084F9080UL,
        [','] = 0x00088400000UL,
        ['-'] = 0x000000F8000UL,
        ['.'] = 0x00018C00000UL,
        ['/'] = 0x00000111110UL,
        [':'] = 0x00018C03180UL,
        [';'] = 0x00088403180UL,
        ['='] = 0x000000F83E0UL,
        ['?'] = 0x0010044422EUL,
        ['['] = 0x0038421084EUL,
        ['\\'] = 0x00001041041UL,
        [']'] = 0x0039084210EUL,
        ['_'] = 0x007C0000000UL,
        ['`'] = 0x00000000044UL,
        ['0'] = 0x003A33AE62EUL,
        ['1'] = 0x003884210C4UL,
        ['2'] = 0x007C444422EUL,
        ['3'] = 0x003E107420FUL,
        ['4'] = 0x00211F4A988UL,
        ['5'] = 0x003E107843FUL,
        ['6'] = 0x003A317842EUL,
        ['7'] = 0x0008422221FUL,
        ['8'] = 0x003A317462EUL,
        ['9'] = 0x003A10F462EUL,
        ['A'] = 0x004631FC62EUL,
        ['B'] = 0x003E317C62FUL,
        ['C'] = 0x003A210862EUL,
        ['D'] = 0x003E318C62FUL,
        ['E'] = 0x007C217843FUL,
        ['F'] = 0x0004217843FUL,
        ['G'] = 0x003A31E862EUL,
        ['H'] = 0x004631FC631UL,
        ['I'] = 0x0038842108EUL,
        ['J'] = 0x0019294211CUL,
        ['K'] = 0x00452519531UL,
        ['L'] = 0x007C2108421UL,
        ['M'] = 0x0046318D771UL,
        ['N'] = 0x004631CD671UL,
        ['O'] = 0x003A318C62EUL,
        ['P'] = 0x0004217C62FUL,
        ['Q'] = 0x0059358C62EUL,
        ['R'] = 0x0045257C62FUL,
        ['S'] = 0x003E107043EUL,
        ['T'] = 0x0010842109FUL,
        ['U'] = 0x003A318C631UL,
        ['V'] = 0x00114A8C631UL,
        ['W'] = 0x002955AC631UL,
        ['X'] = 0x00462A22A31UL,
        ['Y'] = 0x00108422A31UL,
        ['Z'] = 0x007C222221FUL,
        ['a'] = 0x007A3E83800UL,
        ['b'] = 0x0036719B421UL,
        ['c'] = 0x003A218B800UL,
        ['d'] = 0x007A318FA10UL,
        ['e'] = 0x00383F8B800UL,
        ['f'] = 0x00084278A4CUL,
        ['g'] = 0x003A1E8C7C0UL,
        ['h'] = 0x0046319B421UL,
        ['i'] = 0x00388421804UL,
        ['j'] = 0x00192843008UL,
        ['k'] = 0x0024A32A421UL,
        ['l'] = 0x00388421086UL,
        ['m'] = 0x0046B5AAC00UL,
        ['n'] = 0x0046319B400UL,
        ['o'] = 0x003A318B800UL,
        ['p'] = 0x00042F9B400UL,
        ['q'] = 0x00421E8F800UL,
        ['r'] = 0x0004219B400UL,
        ['s'] = 0x003E0E0F800UL,
        ['t'] = 0x00608423C84UL,
        ['u'] = 0x007A318C400UL,
        ['v'] = 0x0011518C400UL,
        ['w'] = 0x002955AC400UL,
        ['x'] = 0x00454454400UL,
        ['y'] = 0x003A1E8C400UL,
        ['z'] = 0x007C4447C00UL,
    };

    public static IReadOnlyCollection<char> PatternKeys => Patterns.Keys;

    public static bool TryGetMask(char c, out ulong mask)
    {
        if (Patterns.TryGetValue(c, out mask))
        {
            return true;
        }

        return Patterns.TryGetValue(char.ToUpperInvariant(c), out mask);
    }

    private static string[] DecodeMask(ulong mask)
    {
        var rows = new string[Height];
        for (var row = 0; row < Height; row++)
        {
            var rowMask = (mask >> (row * Width)) & 0b11111UL;
            var chars = new char[Width];
            for (var col = 0; col < Width; col++)
            {
                chars[col] = ((rowMask >> col) & 1UL) == 1UL ? '1' : '0';
            }

            rows[row] = new string(chars);
        }

        return rows;
    }

    public static bool TryGetPattern(char c, out string[] rows)
    {
        if (TryGetMask(c, out var mask))
        {
            rows = DecodeMask(mask);
            return true;
        }

        rows = Array.Empty<string>();
        return false;
    }

    public static bool[] GetMatrixBits(char c)
    {
        if (!TryGetMask(c, out var mask))
        {
            return new bool[Width * Height];
        }

        var bits = new bool[Width * Height];
        for (var row = 0; row < Height; row++)
        {
            var rowMask = (mask >> (row * Width)) & 0b11111UL;
            for (var col = 0; col < Width; col++)
            {
                bits[(row * Width) + col] = ((rowMask >> col) & 1UL) == 1UL;
            }
        }

        return bits;
    }
}
