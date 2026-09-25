namespace Aurora.Desktop.Overlay.Core.Models;

public sealed record OverlayItem(
    Guid Id,
    string Name,
    string MediaPath,
    OverlayPosition Position,
    OverlaySize Size,
    double Opacity = 1.0,
    bool IsVisible = true,
    bool IsLocked = false,
    bool IsClickThrough = false,
    bool IsTopmost = true,
    bool IsPaused = false,
    double PlaybackSpeed = 1.0,
    int FpsLimit = 30)
{
    public static OverlayItem Create(string name, string mediaPath, OverlaySize? size = null) =>
        new(Guid.NewGuid(), name, mediaPath, new OverlayPosition(100, 100),
            size ?? new OverlaySize(320, 320));

    public OverlayItem WithLayout(OverlayPosition position, OverlaySize size) =>
        this with { Position = position.EnsureFinite(), Size = size.EnsureValid() };

    public OverlayItem WithOpacity(double opacity) =>
        this with { Opacity = Math.Clamp(opacity, 0.0, 1.0) };

    public OverlayItem WithPlayback(bool paused, double speed, int fpsLimit) => this with
    {
        IsPaused = paused,
        PlaybackSpeed = Math.Clamp(speed, 0.25, 4.0),
        FpsLimit = Math.Clamp(fpsLimit, 1, 60)
    };
}
