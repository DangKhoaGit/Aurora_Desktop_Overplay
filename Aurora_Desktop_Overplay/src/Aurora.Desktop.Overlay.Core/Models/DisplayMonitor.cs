namespace Aurora.Desktop.Overlay.Core.Models;

public sealed record DisplayMonitor(
    string Id,
    OverlayPosition Position,
    OverlaySize Size,
    OverlayPosition WorkAreaPosition,
    OverlaySize WorkAreaSize,
    uint DpiX,
    uint DpiY,
    bool IsPrimary);
