using System;

namespace SkeuomorphCore;

/// <summary>
/// Represents the shared state and behavior for a display that is driven by a fixed set of segment bits.
/// </summary>
/// <remarks>
/// Derived types provide the display-specific mapping from a character to the segment bit pattern for that model.
/// </remarks>
public abstract class SegmentDisplayModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentDisplayModelBase"/> class.
    /// </summary>
    /// <param name="segmentCount">The number of segments represented by the display.</param>
    protected SegmentDisplayModelBase(int segmentCount)
    {
        SegmentCount = segmentCount;
        Bits = new bool[segmentCount];
    }

    /// <summary>
    /// Gets the number of segments in the model.
    /// </summary>
    public int SegmentCount { get; }

    /// <summary>
    /// Gets the current segment state for the display.
    /// </summary>
    public bool[] Bits { get; protected set; }

    /// <summary>
    /// Clears all segments to the off state.
    /// </summary>
    public virtual void BlankModule()
    {
        Array.Clear(Bits, 0, Bits.Length);
    }

    /// <summary>
    /// Applies the bit pattern for the specified character to the display.
    /// </summary>
    /// <param name="character">The character to render.</param>
    /// <remarks>
    /// If the source mapping does not match the segment count for the model, the module is cleared rather than partially updating state.
    /// </remarks>
    public virtual void SetChar(char character)
    {
        var source = GetBits(character);
        if (source.Length != Bits.Length)
        {
            Array.Clear(Bits, 0, Bits.Length);
            return;
        }

        Array.Copy(source, Bits, Bits.Length);
    }

    /// <summary>
    /// Gets the segment bit pattern for a character in the display-specific encoding.
    /// </summary>
    /// <param name="character">The character to convert to segment bits.</param>
    /// <returns>The bit pattern that represents the character for the display.</returns>
    protected abstract bool[] GetBits(char character);
}
