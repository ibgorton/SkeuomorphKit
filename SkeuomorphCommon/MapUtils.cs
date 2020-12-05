using System.Collections;
using System.Collections.Generic;

namespace SkeuomorphCommon
{
    public static class MapUtils
    {
        private static readonly Dictionary<char, ushort> RectangleMap = new()
        {

        };

        private static readonly Dictionary<char, byte> SevenMap = new()
        {
            { '-', 64 },
            { '0', 63 },
            { '1', 6 },
            { '2', 91 },
            { '3', 79 },
            { '4', 102 },
            { '5', 109 },
            { '6', 125 },
            { '7', 7 },
            { '8', 127 },
            { '9', 111 },
            { '=', 72 },
            { 'A', 119 },
            { 'B', 124 },
            { 'C', 57 },
            { 'D', 94 },
            { 'E', 121 },
            { 'F', 113 },
            { 'G', 61 },
            { 'H', 118 },
            { '_', 8 },
            { 'r', 80 },
            { 'o', 92 }
        };

        private static readonly Dictionary<char, ushort> SixteenMap = new()
        {
            { ' ', 0 },
            { '!', 12 },
            { '"', 516 },
            { '(', 5120 },
            { ')', 16640 },
            { '*', 65280 },
            { '+', 43520 },
            { ',', 16384 },
            { '-', 34816 },
            { '/', 17408 },
            { '0', 17663 },
            { '1', 1036 },
            { '2', 34935 },
            { '3', 2111 },
            { '4', 34956 },
            { '5', 37043 },
            { '6', 35067 },
            { '7', 15 },
            { '8', 35071 },
            { '9', 35007 },
            { ':', 8704 },
            { ';', 16896 },
            { '<', 37888 },
            { '=', 34864 },
            { '>', 18688 },
            { '?', 10247 },
            { '@', 2807 },
            { 'A', 35023 },
            { 'B', 10815 },
            { 'C', 243 },
            { 'D', 8767 },
            { 'E', 33011 },
            { 'F', 32963 },
            { 'G', 2299 },
            { 'H', 35020 },
            { 'I', 8755 },
            { 'J', 124 },
            { 'K', 38080 },
            { 'L', 240 },
            { 'M', 1484 },
            { 'N', 4556 },
            { 'O', 255 },
            { 'P', 35015 },
            { 'Q', 4351 },
            { 'R', 39111 },
            { 'S', 35003 },
            { 'T', 8707 },
            { 'U', 252 },
            { 'V', 17600 },
            { 'W', 20684 },
            { 'X', 21760 },
            { 'Y', 35004 },
            { 'Z', 17459 },
            { '[', 8722 },
            { '\\', 4352 },
            { ']', 8737 },
            { '^', 20480 },
            { '_', 48 },
            { '`', 256 },
            { 'a', 41072 },
            { 'b', 41184 },
            { 'c', 32864 },
            { 'd', 10268 },
            { 'e', 49248 },
            { 'f', 43522 },
            { 'g', 41633 },
            { 'h', 41152 },
            { 'i', 8192 },
            { 'j', 8800 },
            { 'k', 13824 },
            { 'l', 192 },
            { 'm', 43080 },
            { 'n', 41024 },
            { 'o', 41056 },
            { 'p', 33473 },
            { 'q', 41601 },
            { 'r', 32832 },
            { 's', 41121 },
            { 't', 32992 },
            { 'u', 8288 },
            { 'v', 16448 },
            { 'w', 20552 },
            { 'x', 21760 },
            { 'y', 2588 },
            { 'z', 49184 },
            { '{', 41490 },
            { '|', 8704 },
            { '}', 10785 }
        };

        public static BitArray Rectangle5x7Bits(this char c) => RectangleMap.ContainsKey(c) ? ParseChar(c, RectangleMap) : new BitArray(16);

        public static BitArray SixteenSegmentBits(this char c) => SixteenMap.ContainsKey(c) ? ParseChar(c, SixteenMap) : new BitArray(16);

        public static BitArray SevenSegmentBits(this char c) => SevenMap.ContainsKey(c) ? ParseChar(c, SevenMap) : new BitArray(7);

        private static BitArray ParseChar(char c, Dictionary<char, byte> d)
        {
            return new BitArray(bytes: new byte[] { d[c] })
            {
                Length = 7
            };
        }

        private static BitArray ParseChar(char c, Dictionary<char, ushort> d)
        {
            return new BitArray(values: new int[] { d[c] })
            {
                Length = 16
            };
        }
    }
}
