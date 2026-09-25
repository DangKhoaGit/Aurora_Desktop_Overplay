namespace Aurora.Desktop.Overlay.Core.Models;

public sealed record OverlayConfiguration(
    int SchemaVersion,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<OverlayItem> Overlays)
{
    public const int CurrentSchemaVersion = 1;

    public static OverlayConfiguration Create(IEnumerable<OverlayItem> overlays) =>
        new(CurrentSchemaVersion, DateTimeOffset.UtcNow, overlays.ToArray());
}
