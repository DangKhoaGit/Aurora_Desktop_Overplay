using Aurora.Desktop.Overlay.Core.Interfaces;

namespace Aurora.Desktop.Overlay.Infrastructure;

public sealed class ApplicationPaths : IApplicationPaths
{
    public string DataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AuroraDesktopOverlay");

    public string MediaDirectory => Path.Combine(DataDirectory, "media");
    public string BackupDirectory => Path.Combine(DataDirectory, "backups");
    public string ConfigurationFile => Path.Combine(DataDirectory, "config.json");
    public string ConfigurationBackupFile => Path.Combine(BackupDirectory, "config.backup.json");
    public string LogDirectory => Path.Combine(DataDirectory, "logs");
}
