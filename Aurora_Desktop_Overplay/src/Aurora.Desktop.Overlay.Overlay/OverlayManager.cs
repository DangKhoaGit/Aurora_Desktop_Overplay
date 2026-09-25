using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;
using System.IO;

namespace Aurora.Desktop.Overlay.Overlay;

public sealed class OverlayManager : IOverlayService
{
    private readonly Dictionary<Guid, OverlayItem> _overlays = [];
    public IReadOnlyCollection<OverlayItem> ActiveOverlays => _overlays.Values;

    public OverlayItem Create(string mediaPath, OverlaySize? size = null, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaPath);
        var displayName = string.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(mediaPath) : name.Trim();
        var item = OverlayItem.Create(displayName, mediaPath, size);
        _overlays.Add(item.Id, item);
        return item;
    }

    public bool TryGet(Guid id, out OverlayItem? item) => _overlays.TryGetValue(id, out item);

    public bool Restore(OverlayItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_overlays.ContainsKey(item.Id)) return false;
        item.Position.EnsureFinite();
        item.Size.EnsureValid();
        _overlays.Add(item.Id, item.WithOpacity(item.Opacity)
            .WithPlayback(item.IsPaused, item.PlaybackSpeed, item.FpsLimit));
        return true;
    }

    public OverlayItem? Duplicate(Guid id, OverlayPosition offset)
    {
        if (!_overlays.TryGetValue(id, out var source)) return null;
        var copy = source with
        {
            Id = Guid.NewGuid(),
            Name = $"{source.Name} copy",
            Position = new OverlayPosition(source.Position.X + offset.X, source.Position.Y + offset.Y)
        };
        _overlays.Add(copy.Id, copy);
        return copy;
    }

    public bool Update(OverlayItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_overlays.ContainsKey(item.Id)) return false;
        _overlays[item.Id] = item;
        return true;
    }

    public bool Remove(Guid id) => _overlays.Remove(id);
}
