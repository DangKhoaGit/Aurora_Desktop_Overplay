using Aurora.Desktop.Overlay.Overlay;
using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class OverlayManagerTests
{
    [Fact]
    public void CreateRegistersOverlayWithSafeDefaults()
    {
        var manager = new OverlayManager();
        var result = manager.Create("sample.png");
        Assert.Single(manager.ActiveOverlays);
        Assert.Equal("sample", result.Name);
        Assert.True(result.IsTopmost);
        Assert.Equal(1.0, result.Opacity);
        Assert.Equal(new OverlayPosition(100, 100), result.Position);
    }

    [Fact]
    public void CreateUsesTrimmedCustomName()
    {
        var manager = new OverlayManager();

        var result = manager.Create("sample.png", name: "  Reference image  ");

        Assert.Equal("Reference image", result.Name);
    }

    [Fact]
    public void WithNameRejectsBlankNamesAndTrimsValidNames()
    {
        var item = OverlayItem.Create("original", "sample.png");

        Assert.Equal("renamed", item.WithName("  renamed  ").Name);
        Assert.Throws<ArgumentException>(() => item.WithName("   "));
    }

    [Fact]
    public void RemoveDeletesExistingOverlay()
    {
        var manager = new OverlayManager();
        var item = manager.Create("sample.png");
        Assert.True(manager.Remove(item.Id));
        Assert.Empty(manager.ActiveOverlays);
    }

    [Fact]
    public void UpdateReplacesExistingOverlayState()
    {
        var manager = new OverlayManager();
        var item = manager.Create("sample.png");
        var updated = item.WithLayout(new OverlayPosition(20, 30), new OverlaySize(640, 360)).WithOpacity(0.4);

        Assert.True(manager.Update(updated));
        Assert.True(manager.TryGet(item.Id, out var stored));
        Assert.Equal(updated, stored);
    }

    [Fact]
    public void DuplicateCreatesIndependentOffsetCopy()
    {
        var manager = new OverlayManager();
        var source = manager.Create("sample.png", new OverlaySize(320, 180));

        var copy = manager.Duplicate(source.Id, new OverlayPosition(24, 24));

        Assert.NotNull(copy);
        Assert.NotEqual(source.Id, copy.Id);
        Assert.Equal("sample copy", copy.Name);
        Assert.Equal(new OverlayPosition(124, 124), copy.Position);
        Assert.Equal(source.Size, copy.Size);
        Assert.Equal(2, manager.ActiveOverlays.Count);
    }

    [Fact]
    public void ManagerHandlesTwentyIndependentOverlayModels()
    {
        var manager = new OverlayManager();
        var items = Enumerable.Range(1, 20).Select(index => manager.Create($"sample-{index}.png")).ToArray();

        Assert.Equal(20, manager.ActiveOverlays.Count);
        Assert.All(items, item => Assert.True(manager.TryGet(item.Id, out _)));
        Assert.All(items, item => Assert.True(manager.Remove(item.Id)));
        Assert.Empty(manager.ActiveOverlays);
    }

    [Fact]
    public void PlaybackSettingsAreClampedToSafeRange()
    {
        var item = OverlayItem.Create("animated", "sample.gif").WithPlayback(true, 20, 200);

        Assert.True(item.IsPaused);
        Assert.Equal(4.0, item.PlaybackSpeed);
        Assert.Equal(60, item.FpsLimit);
    }

    [Fact]
    public void RestorePreservesIdentityAndRejectsDuplicate()
    {
        var manager = new OverlayManager();
        var item = OverlayItem.Create("restored", "stored.png") with { IsLocked = true };

        Assert.True(manager.Restore(item));
        Assert.False(manager.Restore(item));
        Assert.True(manager.TryGet(item.Id, out var restored));
        Assert.True(restored?.IsLocked);
    }
}
