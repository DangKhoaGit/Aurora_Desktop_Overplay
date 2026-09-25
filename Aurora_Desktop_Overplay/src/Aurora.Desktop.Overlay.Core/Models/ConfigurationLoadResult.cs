namespace Aurora.Desktop.Overlay.Core.Models;

public sealed record ConfigurationLoadResult(
    OverlayConfiguration Configuration,
    bool RecoveredFromBackup,
    string? Warning = null);
