using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface IMonitorService
{
    IReadOnlyList<DisplayMonitor> GetMonitors();
    DisplayMonitor GetNearestMonitor(nint windowHandle);
    OverlayPosition EnsureVisible(OverlayPosition position, OverlaySize size, nint windowHandle);
}
