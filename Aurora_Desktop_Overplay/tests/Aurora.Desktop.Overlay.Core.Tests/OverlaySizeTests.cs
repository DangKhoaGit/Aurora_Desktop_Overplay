using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class OverlaySizeTests
{
    [Theory]
    [InlineData(1920, 1080, 320, 180)]
    [InlineData(800, 800, 320, 320)]
    public void FitWithinPreservesAspectRatio(double sourceWidth, double sourceHeight,
        double expectedWidth, double expectedHeight)
    {
        var result = OverlaySize.FitWithin(sourceWidth, sourceHeight);
        Assert.Equal(expectedWidth, result.Width, 3);
        Assert.Equal(expectedHeight, result.Height, 3);
    }

    [Fact]
    public void EnsureValidRejectsDimensionsBelowMinimum()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new OverlaySize(20, 100).EnsureValid());
    }
}
