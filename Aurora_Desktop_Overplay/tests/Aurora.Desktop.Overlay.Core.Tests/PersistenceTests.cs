using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;
using Aurora.Desktop.Overlay.Media;
using Aurora.Desktop.Overlay.Persistence;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class PersistenceTests
{
    [Fact]
    public async Task SaveAndLoadRoundTripsConfiguration()
    {
        using var paths = new TemporaryApplicationPaths();
        using var repository = new JsonOverlayConfigurationRepository(paths);
        var expected = OverlayItem.Create("test", "stored.png") with { IsClickThrough = true };

        await repository.SaveAsync(OverlayConfiguration.Create([expected]));
        var result = await repository.LoadAsync();

        Assert.False(result.RecoveredFromBackup);
        Assert.Equal(expected, Assert.Single(result.Configuration.Overlays));
        Assert.False(File.Exists($"{paths.ConfigurationFile}.tmp"));
    }

    [Fact]
    public async Task LoadRecoversPreviousAtomicBackupWhenMainFileIsCorrupt()
    {
        using var paths = new TemporaryApplicationPaths();
        using var repository = new JsonOverlayConfigurationRepository(paths);
        var first = OverlayItem.Create("first", "first.png");
        var second = OverlayItem.Create("second", "second.png");
        await repository.SaveAsync(OverlayConfiguration.Create([first]));
        await repository.SaveAsync(OverlayConfiguration.Create([second]));
        await File.WriteAllTextAsync(paths.ConfigurationFile, "{ corrupt json");

        var result = await repository.LoadAsync();

        Assert.True(result.RecoveredFromBackup);
        Assert.Equal(first, Assert.Single(result.Configuration.Overlays));
    }

    [Fact]
    public async Task MediaImporterCopiesSourceIntoManagedMediaDirectory()
    {
        using var paths = new TemporaryApplicationPaths();
        var source = Path.Combine(paths.DataDirectory, "source.png");
        Directory.CreateDirectory(paths.DataDirectory);
        await File.WriteAllBytesAsync(source, [1, 2, 3]);

        var imported = await new MediaImporter(paths).ImportAsync(source);

        Assert.StartsWith(paths.MediaDirectory, imported, StringComparison.OrdinalIgnoreCase);
        Assert.Equal([1, 2, 3], await File.ReadAllBytesAsync(imported));
        Assert.NotEqual(source, imported);
    }

    [Fact]
    public async Task ConcurrentSavesRemainAtomicAndLoadable()
    {
        using var paths = new TemporaryApplicationPaths();
        using var repository = new JsonOverlayConfigurationRepository(paths);
        var saves = Enumerable.Range(1, 8).Select(index => repository.SaveAsync(
            OverlayConfiguration.Create([OverlayItem.Create($"item-{index}", $"{index}.png")])));

        await Task.WhenAll(saves);
        var result = await repository.LoadAsync();

        Assert.Single(result.Configuration.Overlays);
        Assert.False(File.Exists($"{paths.ConfigurationFile}.tmp"));
    }

    private sealed class TemporaryApplicationPaths : IApplicationPaths, IDisposable
    {
        public TemporaryApplicationPaths() =>
            DataDirectory = Path.Combine(Path.GetTempPath(), "ado-tests", Guid.NewGuid().ToString("N"));

        public string DataDirectory { get; }
        public string MediaDirectory => Path.Combine(DataDirectory, "media");
        public string BackupDirectory => Path.Combine(DataDirectory, "backups");
        public string ConfigurationFile => Path.Combine(DataDirectory, "config.json");
        public string ConfigurationBackupFile => Path.Combine(BackupDirectory, "config.backup.json");
        public string LogDirectory => Path.Combine(DataDirectory, "logs");

        public void Dispose()
        {
            if (Directory.Exists(DataDirectory)) Directory.Delete(DataDirectory, true);
        }
    }
}
