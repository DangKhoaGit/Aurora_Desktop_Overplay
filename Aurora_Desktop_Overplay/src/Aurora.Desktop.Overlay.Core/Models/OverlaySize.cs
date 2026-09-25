namespace Aurora.Desktop.Overlay.Core.Models;

public readonly record struct OverlaySize(double Width, double Height)
{
    public const double MinimumDimension = 48;

    public OverlaySize EnsureValid()
    {
        if (!double.IsFinite(Width) || !double.IsFinite(Height) ||
            Width < MinimumDimension || Height < MinimumDimension)
        {
            throw new ArgumentOutOfRangeException(nameof(Width),
                $"Overlay dimensions must be finite and at least {MinimumDimension}px.");
        }

        return this;
    }

    public static OverlaySize FitWithin(double sourceWidth, double sourceHeight, double maximumDimension = 320)
    {
        if (sourceWidth <= 0 || sourceHeight <= 0 || maximumDimension < MinimumDimension)
            throw new ArgumentOutOfRangeException(nameof(sourceWidth));

        var scale = maximumDimension / Math.Max(sourceWidth, sourceHeight);
        if (Math.Min(sourceWidth, sourceHeight) * scale < MinimumDimension)
            scale = MinimumDimension / Math.Min(sourceWidth, sourceHeight);
        return new OverlaySize(sourceWidth * scale, sourceHeight * scale).EnsureValid();
    }
}
