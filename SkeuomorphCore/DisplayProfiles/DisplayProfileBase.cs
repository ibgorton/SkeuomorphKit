using System;
using System.Collections.Generic;

namespace SkeuomorphCore
{
    public abstract class DisplayProfileBase : IDisplayProfile
    {
        private readonly HashSet<char> _supportedCharacters;

        protected DisplayProfileBase(string name, int segmentCount, int width, int height, IEnumerable<char> supportedCharacters)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SegmentCount = segmentCount;
            Width = width;
            Height = height;
            _supportedCharacters = new HashSet<char>(supportedCharacters);
        }

        public string Name { get; }

        public int SegmentCount { get; }

        public int Width { get; }

        public int Height { get; }

        public virtual bool IsSupported(char c)
        {
            return _supportedCharacters.Contains(c) || _supportedCharacters.Contains(char.ToUpperInvariant(c));
        }

        public abstract bool[] GetBits(char c);
    }
}
