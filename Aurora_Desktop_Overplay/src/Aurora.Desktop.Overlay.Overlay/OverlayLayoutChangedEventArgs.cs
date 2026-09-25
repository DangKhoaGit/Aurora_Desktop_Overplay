using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Overlay;

public sealed class OverlayLayoutChangedEventArgs(OverlayPosition position, OverlaySize size) : EventArgs
{
    public OverlayPosition Position { get; } = position;
    public OverlaySize Size { get; } = size;
}
