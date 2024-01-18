using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeuomorphCommon
{
    public static class SevenMap
    {
        private static readonly Dictionary<char, bool[]> SevenBits = new()
        {
            { '-', new bool[7] { true, false, false, false, false, false, false } },
            { '0', new bool[7] { false, true, true, true, true, true, true, } },
            { '1', new bool[7] { false, false, false, false, true, true, false } },
            { '2', new bool[7] { true, false, true, true, false, true, true } },
            { '3', new bool[7] { true, false, false, true, true, true, true } },
            { '4', new bool[7] { true, true, false, false, true, true, false} },
            { '5', new bool[7] { true, true, false, true, true, false, true } },
            { '6', new bool[7] { true, true, true, true, true, false, true } },
            { '7', new bool[7] { false, false, false, false, true, true, true} },
            { '8', new bool[7] { true, true, true, true, true, true, true} },
            { '9', new bool[7] { true, true, false, true, true, true, true } },
            { '=', new bool[7] { true, false, false, true, false, false, false } },
            { 'A', new bool[7] { true, true, true, false, true, true, true } },
            { 'B', new bool[7] { true, true, true, true, true, false, false } },
            { 'C', new bool[7] { false, true, true, true, false, false, true } },
            { 'D', new bool[7] { true, false, true, true, true, true, false } },
            { 'E', new bool[7] { true, true, true, true, false, false, true } },
            { 'F', new bool[7] { true, true, true, false, false, false, true } },
            { 'G', new bool[7] { false, true, true, true, true, false, true } },
            { 'H', new bool[7] { true, true, true, false, true, true, false } },
            { '_', new bool[7] { false, false, false, true, false, false, false } },
            { 'r', new bool[7] { true, false, true, false, false, false, false } },
            { 'o', new bool[7] { true, false, true, true, true, false, false} }
        };

        public static void GetBitSeven(this bool[] t, char c)
        {
            if (SevenBits.ContainsKey(key: c))
            {
                SevenBits[key: c].Reverse().ToArray().CopyTo(array: t, index: 0);
                //SevenBits[key: c].CopyTo(array: t, index: 0);
            }
        }

    }
}