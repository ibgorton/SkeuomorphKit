namespace SkeuomorphCore;

// A 14-segment display follows the standard Lite-On hardware convention: the 16-segment
// canonical set is reduced by omitting the non-existent I and O segments while keeping the
// actual A/B/C/D/E/F/G/H/J/K/L/M/N/P ordering used by real 14-seg displays.
public static class FourteenMap
{
    private const int SegmentCount = 14;

    // Canonical 14-segment order for real Lite-On-style hardware:
    // A,B,C,D,E,F,G,H,J,K,L,M,N,P.
    // I and O are omitted because they do not exist on the common 14-seg subset.
    public const ushort SegmentA = 0x0001;
    public const ushort SegmentB = 0x0002;
    public const ushort SegmentC = 0x0004;
    public const ushort SegmentD = 0x0008;
    public const ushort SegmentE = 0x0010;
    public const ushort SegmentF = 0x0020;
    public const ushort SegmentG = 0x0040;
    public const ushort SegmentH = 0x0080;
    public const ushort SegmentJ = 0x0100;
    public const ushort SegmentK = 0x0200;
    public const ushort SegmentL = 0x0400;
    public const ushort SegmentM = 0x0800;
    public const ushort SegmentN = 0x1000;
    public const ushort SegmentP = 0x2000;

    public static readonly string[] SegmentLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P"];

    public static bool[] GetBitsFourteen(this char c)
    {
        var supported = DisplayCharacterProfiles.IsSupported("FourteenSegment", c);
        if (!supported)
        {
            return new bool[SegmentCount];
        }

        var sixteen = c.GetBitsSixteen();
        var result = new bool[SegmentCount];

        // Canonical 16-seg order: A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P.
        // Real 14-seg hardware omits I and O, leaving A,B,C,D,E,F,G,H,J,K,L,M,N,P.
        var mapping = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 15 };
        for (var i = 0; i < SegmentCount; i++)
        {
            result[i] = sixteen[mapping[i]];
        }

        return result;
    }
}
