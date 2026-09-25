using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface IOverlayService
{
    IReadOnlyCollection<OverlayItem> ActiveOverlays { get; }
    OverlayItem Create(string mediaPath, OverlaySize? size = null, string? name = null);
    bool Restore(OverlayItem item);
    OverlayItem? Duplicate(Guid id, OverlayPosition offset);
    bool TryGet(Guid id, out OverlayItem? item);
    bool Update(OverlayItem item);
    bool Remove(Guid id);
}
