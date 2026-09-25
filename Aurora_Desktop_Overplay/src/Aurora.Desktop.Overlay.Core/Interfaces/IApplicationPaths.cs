namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface IApplicationPaths
{
    string DataDirectory { get; }
    string MediaDirectory { get; }
    string BackupDirectory { get; }
    string ConfigurationFile { get; }
    string ConfigurationBackupFile { get; }
    string LogDirectory { get; }
}
