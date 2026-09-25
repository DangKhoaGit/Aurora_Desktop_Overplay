using Aurora.Desktop.Overlay.Media;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class StaticImageLoaderTests
{
    [Fact]
    public void LoadReadsGifFramesAndTiming()
    {
        byte[] animatedGif =
        [
            0x47,0x49,0x46,0x38,0x39,0x61,0x01,0x00,0x01,0x00,0x80,0x00,0x00,
            0x00,0x00,0x00,0xFF,0xFF,0xFF,
            0x21,0xF9,0x04,0x00,0x05,0x00,0x00,0x00,
            0x2C,0x00,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x02,0x02,0x44,0x01,0x00,
            0x21,0xF9,0x04,0x00,0x0A,0x00,0x00,0x00,
            0x2C,0x00,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x02,0x02,0x44,0x01,0x00,
            0x3B
        ];
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.gif");
        try
        {
            File.WriteAllBytes(path, animatedGif);
            var result = new StaticImageLoader().Load(path);

            Assert.True(result.IsAnimated);
            Assert.Equal(2, result.Frames.Count);
            Assert.Equal(TimeSpan.FromMilliseconds(50), result.FrameDurations[0]);
            Assert.Equal(TimeSpan.FromMilliseconds(100), result.FrameDurations[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadWrapsCorruptImageAsInvalidDataError()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.gif");
        try
        {
            File.WriteAllBytes(path, [0x47, 0x49, 0x46]);
            Assert.Throws<InvalidDataException>(() => new StaticImageLoader().Load(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
