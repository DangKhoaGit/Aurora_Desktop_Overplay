using Aurora.Desktop.Overlay.Platform.Windows;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class MonitorServiceTests
{
    [Fact]
    public void GetMonitorsReturnsValidWindowsDisplayData()
    {
        var monitors = new MonitorService().GetMonitors();

        Assert.NotEmpty(monitors);
        Assert.All(monitors, monitor =>
        {
            Assert.True(monitor.Size.Width > 0);
            Assert.True(monitor.Size.Height > 0);
            Assert.True(monitor.DpiX > 0);
            Assert.True(monitor.DpiY > 0);
        });
    }
}
