namespace SkeuomorphCore;

public interface IDisplayProfile
{
    string Name { get; }
    int SegmentCount { get; }
    int Width { get; }
    int Height { get; }
    bool IsSupported(char c);
    bool[] GetBits(char c);
}
