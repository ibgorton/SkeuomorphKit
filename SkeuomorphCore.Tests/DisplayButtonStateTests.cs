using SkeuomorphCore;

namespace SkeuomorphCore.Tests;

public class DisplayButtonStateTests
{
    [Fact]
    public void FromValue_WhenAtMaximum_UpButtonDisabled()
    {
        var state = DisplayButtonState.FromValue(10d, 0d, 10d);

        Assert.True(state.UpDisabled);
        Assert.False(state.DownDisabled);
    }

    [Fact]
    public void FromValue_WhenAtMinimum_DownButtonDisabled()
    {
        var state = DisplayButtonState.FromValue(0d, 0d, 10d);

        Assert.False(state.UpDisabled);
        Assert.True(state.DownDisabled);
    }

    [Fact]
    public void FromValue_WhenInsideRange_BothButtonsEnabled()
    {
        var state = DisplayButtonState.FromValue(5d, 0d, 10d);

        Assert.False(state.UpDisabled);
        Assert.False(state.DownDisabled);
    }

    [Fact]
    public void FromValue_WhenAboveMaximum_UpButtonDisabled()
    {
        var state = DisplayButtonState.FromValue(11d, 0d, 10d);

        Assert.True(state.UpDisabled);
        Assert.False(state.DownDisabled);
    }

    [Fact]
    public void FromValue_WhenBelowMinimum_DownButtonDisabled()
    {
        var state = DisplayButtonState.FromValue(-1d, 0d, 10d);

        Assert.False(state.UpDisabled);
        Assert.True(state.DownDisabled);
    }
}
