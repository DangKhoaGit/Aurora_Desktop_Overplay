namespace Aurora.Desktop.Overlay.Core.Models;

public readonly record struct OverlayPosition(double X, double Y)
{
    public OverlayPosition EnsureFinite() =>
        double.IsFinite(X) && double.IsFinite(Y)
            ? this
            : throw new ArgumentOutOfRangeException(nameof(X), "Overlay coordinates must be finite.");
}
