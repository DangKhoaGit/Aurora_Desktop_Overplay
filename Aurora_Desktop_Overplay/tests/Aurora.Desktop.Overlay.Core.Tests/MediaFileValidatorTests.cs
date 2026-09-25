using Aurora.Desktop.Overlay.Media;

namespace Aurora.Desktop.Overlay.Core.Tests;

public sealed class MediaFileValidatorTests
{
    [Fact]
    public void ValidateRejectsMissingFile()
    {
        var validator = new MediaFileValidator();
        var result = validator.Validate(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png"));
        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidateRejectsUnsupportedExtension()
    {
        var path = Path.GetTempFileName();
        try
        {
            var result = new MediaFileValidator().Validate(path);
            Assert.False(result.IsValid);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ValidateRejectsFileAboveConfiguredLimit()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.png");
        try
        {
            File.WriteAllBytes(path, [0, 1]);
            var result = new MediaFileValidator(1).Validate(path);
            Assert.False(result.IsValid);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
