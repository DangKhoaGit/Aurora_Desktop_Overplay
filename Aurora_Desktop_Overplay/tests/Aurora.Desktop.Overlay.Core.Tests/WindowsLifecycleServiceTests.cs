using Aurora.Desktop.Overlay.Infrastructure;
using Aurora.Desktop.Overlay.Platform.Windows;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class WindowsLifecycleServiceTests
{
    [Fact]
    public void SecondInstanceSignalsPrimaryInstance()
    {
        var key = $"ADO.Tests.{Guid.NewGuid():N}";
        using var primary = new SingleInstanceService(key);
        using var secondary = new SingleInstanceService(key);
        using var activated = new ManualResetEventSlim();

        Assert.True(primary.TryAcquire());
        primary.ActivationRequested += (_, _) => activated.Set();
        primary.StartListening();
        Assert.False(secondary.TryAcquire());
        secondary.SignalExistingInstance();

        Assert.True(activated.Wait(TimeSpan.FromSeconds(2)));
    }

    [Fact]
    public void FileLoggerWritesInformationToDailyLog()
    {
        using var paths = new TemporaryApplicationPaths();
        using var provider = new FileLoggerProvider(paths);
        var logger = provider.CreateLogger("ADO.Tests");

        logger.Log(LogLevel.Information, new EventId(1, "Test"), "Lifecycle test message", null,
            static (state, _) => state);

        var log = Assert.Single(Directory.GetFiles(paths.LogDirectory, "*.log"));
        Assert.Contains("Lifecycle test message", File.ReadAllText(log), StringComparison.Ordinal);
    }

    [Fact]
    public void StartupCommandQuotesExecutableAndIncludesDelay()
    {
        var command = WindowsStartupService.BuildCommand(@"C:\Program Files\ADO\ado.exe", 5);

        Assert.Equal("\"C:\\Program Files\\ADO\\ado.exe\" --minimized --startup-delay=5", command);
    }

    private sealed class TemporaryApplicationPaths : IApplicationPaths, IDisposable
    {
        public TemporaryApplicationPaths() =>
            DataDirectory = Path.Combine(Path.GetTempPath(), "ado-lifecycle-tests", Guid.NewGuid().ToString("N"));

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
