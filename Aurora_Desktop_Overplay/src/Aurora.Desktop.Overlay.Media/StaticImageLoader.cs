using System.Windows.Media.Imaging;
using System.IO;

namespace Aurora.Desktop.Overlay.Media;

public sealed class StaticImageLoader(
    long maximumPixelCount = 80_000_000,
    long maximumDecodedPixelCount = 160_000_000)
{
    public LoadedImage Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);
            var firstFrame = decoder.Frames[0];
            if ((long)firstFrame.PixelWidth * firstFrame.PixelHeight > maximumPixelCount)
                throw new InvalidDataException("The image dimensions exceed the configured safety limit.");
            var frames = new List<BitmapSource>(decoder.Frames.Count);
            var durations = new List<TimeSpan>(decoder.Frames.Count);
            long decodedPixelCount = 0;
            foreach (var frame in decoder.Frames)
            {
                decodedPixelCount = checked(decodedPixelCount + ((long)frame.PixelWidth * frame.PixelHeight));
                if (decodedPixelCount > maximumDecodedPixelCount)
                    throw new InvalidDataException("The animation contains too many decoded pixels.");
                frame.Freeze();
                frames.Add(frame);
                durations.Add(ReadFrameDuration(frame));
            }
            return new LoadedImage(frames, durations, firstFrame.PixelWidth, firstFrame.PixelHeight);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException or FormatException)
        {
            throw new InvalidDataException("The image could not be decoded. It may be damaged or unsupported.", exception);
        }
    }

    private static TimeSpan ReadFrameDuration(BitmapFrame frame)
    {
        const int defaultDelayMilliseconds = 100;
        if (frame.Metadata is not BitmapMetadata metadata)
            return TimeSpan.FromMilliseconds(defaultDelayMilliseconds);
        try
        {
            var rawDelay = metadata.GetQuery("/grctlext/Delay");
            var centiseconds = rawDelay switch
            {
                ushort value => value,
                byte value => value,
                _ => 0
            };
            return TimeSpan.FromMilliseconds(Math.Max(20, centiseconds * 10));
        }
        catch (NotSupportedException)
        {
            return TimeSpan.FromMilliseconds(defaultDelayMilliseconds);
        }
    }
}
