using System.Windows.Media.Imaging;

namespace Aurora.Desktop.Overlay.Media;

public sealed record LoadedImage(
    IReadOnlyList<BitmapSource> Frames,
    IReadOnlyList<TimeSpan> FrameDurations,
    int PixelWidth,
    int PixelHeight)
{
    public BitmapSource Source => Frames[0];
    public bool IsAnimated => Frames.Count > 1;
}
