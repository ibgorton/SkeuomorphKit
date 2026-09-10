using System.Collections;
using System.Collections.Generic;

namespace SkeuomorphCore
{
    public static class SixteenMap
    {
        private static readonly Dictionary<char, BitArray> SixteenBits = new()
        {
            { ' ', new BitArray(values: new[] { 0 }) { Length = 16 } },
            { '!', new BitArray(values: new[] { 12 }) { Length = 16 } },
            { '"', new BitArray(values: new[] { 516 }) { Length = 16 } },
            { '(', new BitArray(values: new[] { 5120 }) { Length = 16 } },
            { ')', new BitArray(values: new[] { 16640 }) { Length = 16 } },
            { '*', new BitArray(values: new[] { 65280 }) { Length = 16 } },
            { '+', new BitArray(values: new[] { 43520 }) { Length = 16 } },
            { ',', new BitArray(values: new[] { 16384 }) { Length = 16 } },
            { '-', new BitArray(values: new[] { 34816 }) { Length = 16 } },
            { '/', new BitArray(values: new[] { 17408 }) { Length = 16 } },
            { '0', new BitArray(values: new[] { 17663 }) { Length = 16 } },
            { '1', new BitArray(values: new[] { 1036 }) { Length = 16 } },
            { '2', new BitArray(values: new[] { 34935 }) { Length = 16 } },
            { '3', new BitArray(values: new[] { 2111 }) { Length = 16 } },
            { '4', new BitArray(values: new[] { 34956 }) { Length = 16 } },
            { '5', new BitArray(values: new[] { 37043 }) { Length = 16 } },
            { '6', new BitArray(values: new[] { 35067 }) { Length = 16 } },
            { '7', new BitArray(values: new[] { 15 }) { Length = 16 } },
            { '8', new BitArray(values: new[] { 35071 }) { Length = 16 } },
            { '9', new BitArray(values: new[] { 35007 }) { Length = 16 } },
            { ':', new BitArray(values: new[] { 8704 }) { Length = 16 } },
            { ';', new BitArray(values: new[] { 16896 }) { Length = 16 } },
            { '<', new BitArray(values: new[] { 37888 }) { Length = 16 } },
            { '=', new BitArray(values: new[] { 34864 }) { Length = 16 } },
            { '>', new BitArray(values: new[] { 18688 }) { Length = 16 } },
            { '?', new BitArray(values: new[] { 10247 }) { Length = 16 } },
            { '@', new BitArray(values: new[] { 2807 }) { Length = 16 } },
            { 'A', new BitArray(values: new[] { 35023 }) { Length = 16 } },
            { 'B', new BitArray(values: new[] { 10815 }) { Length = 16 } },
            { 'C', new BitArray(values: new[] { 243 }) { Length = 16 } },
            { 'D', new BitArray(values: new[] { 8767 }) { Length = 16 } },
            { 'E', new BitArray(values: new[] { 33011 }) { Length = 16 } },
            { 'F', new BitArray(values: new[] { 32963 }) { Length = 16 } },
            { 'G', new BitArray(values: new[] { 2299 }) { Length = 16 } },
            { 'H', new BitArray(values: new[] { 35020 }) { Length = 16 } },
            { 'I', new BitArray(values: new[] { 8755 }) { Length = 16 } },
            { 'J', new BitArray(values: new[] { 124 }) { Length = 16 } },
            { 'K', new BitArray(values: new[] { 38080 }) { Length = 16 } },
            { 'L', new BitArray(values: new[] { 240 }) { Length = 16 } },
            { 'M', new BitArray(values: new[] { 1484 }) { Length = 16 } },
            { 'N', new BitArray(values: new[] { 4556 }) { Length = 16 } },
            { 'O', new BitArray(values: new[] { 255 }) { Length = 16 } },
            { 'P', new BitArray(values: new[] { 35015 }) { Length = 16 } },
            { 'Q', new BitArray(values: new[] { 4351 }) { Length = 16 } },
            { 'R', new BitArray(values: new[] { 39111 }) { Length = 16 } },
            { 'S', new BitArray(values: new[] { 35003 }) { Length = 16 } },
            { 'T', new BitArray(values: new[] { 8707 }) { Length = 16 } },
            { 'U', new BitArray(values: new[] { 252 }) { Length = 16 } },
            { 'V', new BitArray(values: new[] { 17600 }) { Length = 16 } },
            { 'W', new BitArray(values: new[] { 20684 }) { Length = 16 } },
            { 'X', new BitArray(values: new[] { 21760 }) { Length = 16 } },
            { 'Y', new BitArray(values: new[] { 35004 }) { Length = 16 } },
            { 'Z', new BitArray(values: new[] { 17459 }) { Length = 16 } },
            { '[', new BitArray(values: new[] { 8722 }) { Length = 16 } },
            { '\\', new BitArray(values: new[] { 4352 }) { Length = 16 } },
            { ']', new BitArray(values: new[] { 8737 }) { Length = 16 } },
            { '^', new BitArray(values: new[] { 20480 }) { Length = 16 } },
            { '_', new BitArray(values: new[] { 48 }) { Length = 16 } },
            { '`', new BitArray(values: new[] { 256 }) { Length = 16 } },
            { 'a', new BitArray(values: new[] { 41072 }) { Length = 16 } },
            { 'b', new BitArray(values: new[] { 41184 }) { Length = 16 } },
            { 'c', new BitArray(values: new[] { 32864 }) { Length = 16 } },
            { 'd', new BitArray(values: new[] { 10268 }) { Length = 16 } },
            { 'e', new BitArray(values: new[] { 49248 }) { Length = 16 } },
            { 'f', new BitArray(values: new[] { 43522 }) { Length = 16 } },
            { 'g', new BitArray(values: new[] { 41633 }) { Length = 16 } },
            { 'h', new BitArray(values: new[] { 41152 }) { Length = 16 } },
            { 'i', new BitArray(values: new[] { 8192 }) { Length = 16 } },
            { 'j', new BitArray(values: new[] { 8800 }) { Length = 16 } },
            { 'k', new BitArray(values: new[] { 13824 }) { Length = 16 } },
            { 'l', new BitArray(values: new[] { 192 }) { Length = 16 } },
            { 'm', new BitArray(values: new[] { 43080 }) { Length = 16 } },
            { 'n', new BitArray(values: new[] { 41024 }) { Length = 16 } },
            { 'o', new BitArray(values: new[] { 41056 }) { Length = 16 } },
            { 'p', new BitArray(values: new[] { 33473 }) { Length = 16 } },
            { 'q', new BitArray(values: new[] { 41601 }) { Length = 16 } },
            { 'r', new BitArray(values: new[] { 32832 }) { Length = 16 } },
            { 's', new BitArray(values: new[] { 41121 }) { Length = 16 } },
            { 't', new BitArray(values: new[] { 32992 }) { Length = 16 } },
            { 'u', new BitArray(values: new[] { 8288 }) { Length = 16 } },
            { 'v', new BitArray(values: new[] { 16448 }) { Length = 16 } },
            { 'w', new BitArray(values: new[] { 20552 }) { Length = 16 } },
            { 'x', new BitArray(values: new[] { 21760 }) { Length = 16 } },
            { 'y', new BitArray(values: new[] { 2588 }) { Length = 16 } },
            { 'z', new BitArray(values: new[] { 49184 }) { Length = 16 } },
            { '{', new BitArray(values: new[] { 41490 }) { Length = 16 } },
            { '|', new BitArray(values: new[] { 8704 }) { Length = 16 } },
            { '}', new BitArray(values: new[] { 10785 }) { Length = 16 } }
        };

        public static bool[] GetBitsSixteen(this char c)
        {
            var key = char.ToUpperInvariant(c);
            if (!GlyphLibrary.TryGetPattern(c, out _))
            {
                return new bool[16];
            }

            if (!SixteenBits.TryGetValue(key, out BitArray bits))
                return new bool[16];

            var result = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = bits.Get(i);
            }

            return result;
        }
    }
}