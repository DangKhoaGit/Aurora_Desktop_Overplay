using System.Windows;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;
using Aurora.Desktop.Overlay.Media;

namespace Aurora.Desktop.Overlay.Overlay;

public sealed class OverlayWindowCoordinator(
    IOverlayService overlayService,
    IWindowStyleService windowStyleService,
    IMonitorService monitorService) : IDisposable
{
    private readonly Dictionary<Guid, WindowEntry> _windows = [];
    private readonly Dictionary<Guid, bool> _clickThroughBeforeEditMode = [];
    private bool _isDisposing;
    private bool _isEditMode;

    public event EventHandler? Changed;

    public IReadOnlyCollection<OverlayItem> Items => overlayService.ActiveOverlays;

    public OverlayItem Create(string mediaPath, LoadedImage image, string? name = null)
    {
        var size = OverlaySize.FitWithin(image.PixelWidth, image.PixelHeight);
        var item = overlayService.Create(mediaPath, size, name);
        Attach(item, image);
        return item;
    }

    public bool Restore(OverlayItem item, LoadedImage image)
    {
        if (!overlayService.Restore(item)) return false;
        Attach(item, image);
        return true;
    }

    public OverlayItem? Duplicate(Guid id)
    {
        if (!_windows.TryGetValue(id, out var source)) return null;
        var copy = overlayService.Duplicate(id, new OverlayPosition(24, 24));
        if (copy is null) return null;
        Attach(copy, source.Image);
        return copy;
    }

    public bool Remove(Guid id)
    {
        if (!_windows.Remove(id, out var entry)) return false;
        entry.Window.Close();
        _clickThroughBeforeEditMode.Remove(id);
        var removed = overlayService.Remove(id);
        OnChanged();
        return removed;
    }

    public bool TryGet(Guid id, out OverlayItem? item) => overlayService.TryGet(id, out item);

    public bool IsAnimated(Guid id) => _windows.TryGetValue(id, out var entry) && entry.Image.IsAnimated;

    public bool Rename(Guid id, string name)
    {
        if (!_windows.ContainsKey(id) || !overlayService.TryGet(id, out var item) || item is null ||
            string.IsNullOrWhiteSpace(name)) return false;
        var renamed = overlayService.Update(item.WithName(name));
        if (renamed) OnChanged();
        return renamed;
    }

    public bool HasVisibleOverlays => overlayService.ActiveOverlays.Any(item => item.IsVisible);

    public void SetVisible(Guid id, bool visible) => Update(id,
        item => item with { IsVisible = visible },
        (window, _) => window.Visibility = visible ? Visibility.Visible : Visibility.Hidden);

    public void SetLocked(Guid id, bool locked) => Update(id,
        item => item with { IsLocked = locked },
        (window, _) => window.SetLocked(locked));

    public void SetClickThrough(Guid id, bool enabled) => Update(id,
        item => item with { IsClickThrough = enabled },
        (window, _) => window.SetClickThrough(enabled));

    public void SetTopmost(Guid id, bool enabled) => Update(id,
        item => item with { IsTopmost = enabled },
        (window, _) => window.SetTopmost(enabled));

    public void SetOpacity(Guid id, double opacity) => Update(id,
        item => item.WithOpacity(opacity),
        (window, item) => window.SetOverlayOpacity(item.Opacity));

    public void SetPlayback(Guid id, bool paused, double speed, int fpsLimit) => Update(id,
        item => item.WithPlayback(paused, speed, fpsLimit),
        (window, item) => window.SetPlayback(item.IsPaused, item.PlaybackSpeed, item.FpsLimit));

    public void ResetPosition(Guid id)
    {
        if (_windows.TryGetValue(id, out var entry)) entry.Window.ResetPosition();
    }

    public void BringToFront(Guid id)
    {
        if (!_windows.TryGetValue(id, out var entry) || !overlayService.TryGet(id, out var item) || item is null)
            return;
        if (!item.IsVisible) SetVisible(id, true);
        entry.Window.BringToFront(item.IsTopmost);
    }

    public void LockAll()
    {
        foreach (var id in _windows.Keys.ToArray()) SetLocked(id, true);
    }

    public void ShowAll()
    {
        foreach (var id in _windows.Keys.ToArray()) SetVisible(id, true);
    }

    public void HideAll()
    {
        foreach (var id in _windows.Keys.ToArray()) SetVisible(id, false);
    }

    public void ToggleAll()
    {
        if (HasVisibleOverlays) HideAll();
        else ShowAll();
    }

    public void SetEditMode(bool enabled)
    {
        if (_isEditMode == enabled) return;
        _isEditMode = enabled;
        foreach (var id in _windows.Keys.ToArray())
        {
            if (!overlayService.TryGet(id, out var item) || item is null) continue;
            if (enabled)
            {
                _clickThroughBeforeEditMode[id] = item.IsClickThrough;
                SetClickThrough(id, false);
                SetLocked(id, false);
            }
            else
            {
                SetLocked(id, true);
                if (_clickThroughBeforeEditMode.Remove(id, out var wasClickThrough))
                    SetClickThrough(id, wasClickThrough);
            }
        }
    }

    public void Dispose()
    {
        _isDisposing = true;
        foreach (var entry in _windows.Values.ToArray()) entry.Window.Close();
        _windows.Clear();
        _clickThroughBeforeEditMode.Clear();
    }

    private void Attach(OverlayItem item, LoadedImage image)
    {
        var window = new OverlayWindow(image, windowStyleService, monitorService)
        {
            Left = item.Position.X,
            Top = item.Position.Y,
            Width = item.Size.Width,
            Height = item.Size.Height,
            Opacity = item.Opacity,
            Topmost = item.IsTopmost
        };
        window.SetLocked(item.IsLocked);
        window.SetPlayback(item.IsPaused, item.PlaybackSpeed, item.FpsLimit);
        window.LayoutChanged += (_, args) => UpdateLayout(item.Id, args);
        window.Closed += (_, _) => HandleWindowClosed(item.Id);
        _windows.Add(item.Id, new WindowEntry(window, image));
        if (_isEditMode) _clickThroughBeforeEditMode[item.Id] = item.IsClickThrough;
        window.Show();
        window.SetClickThrough(item.IsClickThrough);
        if (!item.IsVisible) window.Hide();
        OnChanged();
    }

    private void Update(Guid id, Func<OverlayItem, OverlayItem> change,
        Action<OverlayWindow, OverlayItem> applyToWindow)
    {
        if (!_windows.TryGetValue(id, out var entry) || !overlayService.TryGet(id, out var item) || item is null)
            return;
        var updated = change(item);
        if (!overlayService.Update(updated)) return;
        applyToWindow(entry.Window, updated);
        OnChanged();
    }

    private void UpdateLayout(Guid id, OverlayLayoutChangedEventArgs args)
    {
        if (!overlayService.TryGet(id, out var item) || item is null) return;
        overlayService.Update(item.WithLayout(args.Position, args.Size));
        OnChanged();
    }

    private void HandleWindowClosed(Guid id)
    {
        if (_isDisposing || !_windows.Remove(id)) return;
        _clickThroughBeforeEditMode.Remove(id);
        overlayService.Remove(id);
        OnChanged();
    }

    private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);

    private sealed record WindowEntry(OverlayWindow Window, LoadedImage Image);
}
