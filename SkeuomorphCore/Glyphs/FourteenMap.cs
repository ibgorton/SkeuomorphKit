namespace SkeuomorphCore;

// A 14-segment display follows the standard Lite-On hardware convention: the 16-segment
// canonical set is reduced by omitting the non-existent I and O segments while keeping the
// actual A/B/C/D/E/F/G/H/J/K/L/M/N/P ordering used by real 14-seg displays.
public static class FourteenMap
{
    private const int SegmentCount = 14;

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
