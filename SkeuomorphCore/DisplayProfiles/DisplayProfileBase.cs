using System;
using System.Collections.Generic;

namespace SkeuomorphCore;

/// <summary>
/// Provides the shared behavior for a display profile that defines segment geometry and supported characters.
/// </summary>
/// <remarks>
/// Derived classes describe a specific display layout, such as seven-segment or sixteen-segment, and provide
/// the bit mapping required to render characters on that layout.
/// </remarks>
public abstract class DisplayProfileBase : IDisplayProfile
{
    private readonly HashSet<char> _supportedCharacters;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayProfileBase"/> class.
    /// </summary>
    /// <param name="name">The display profile name.</param>
    /// <param name="segmentCount">The total number of segments in the display.</param>
    /// <param name="width">The display width in logical columns.</param>
    /// <param name="height">The display height in logical rows.</param>
    /// <param name="supportedCharacters">The set of characters supported by the display type.</param>
    protected DisplayProfileBase(string name, int segmentCount, int width, int height, IEnumerable<char> supportedCharacters)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SegmentCount = segmentCount;
        Width = width;
        Height = height;
        _supportedCharacters = new HashSet<char>(supportedCharacters);
    }

    /// <summary>
    /// Gets the display profile name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the total number of segments in the display layout.
    /// </summary>
    public int SegmentCount { get; }

    /// <summary>
    /// Gets the display width in logical cells.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the display height in logical cells.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Determines whether the specified character is supported by this display profile.
    /// </summary>
    /// <param name="c">The character to test.</param>
    /// <returns><c>true</c> if the character is supported; otherwise, <c>false</c>.</returns>
    public virtual bool IsSupported(char c)
    {
        return _supportedCharacters.Contains(c) || _supportedCharacters.Contains(char.ToUpperInvariant(c));
    }

    /// <summary>
    /// Gets the segment bit pattern for a character in this display profile.
    /// </summary>
    /// <param name="c">The character to convert.</param>
    /// <returns>The segment bit pattern for the specified character.</returns>
    public abstract bool[] GetBits(char c);
}
