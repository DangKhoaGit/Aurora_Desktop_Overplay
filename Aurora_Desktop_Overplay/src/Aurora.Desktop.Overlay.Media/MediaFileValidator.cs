using System.IO;

namespace Aurora.Desktop.Overlay.Media;

public sealed class MediaFileValidator(long maximumFileSizeBytes = 50 * 1024 * 1024)
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };

    public MediaValidationResult Validate(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return MediaValidationResult.Failure("No media file was selected.");
        if (!SupportedExtensions.Contains(Path.GetExtension(path)))
            return MediaValidationResult.Failure("Only PNG, JPG, JPEG, BMP, and GIF files are supported.");
        if (!File.Exists(path))
            return MediaValidationResult.Failure("The selected media file no longer exists.");

        try
        {
            var length = new FileInfo(path).Length;
            if (length <= 0) return MediaValidationResult.Failure("The selected media file is empty.");
            if (length > maximumFileSizeBytes)
                return MediaValidationResult.Failure($"The selected media exceeds the {maximumFileSizeBytes / 1024 / 1024} MB limit.");
        }
        catch (IOException)
        {
            return MediaValidationResult.Failure("The selected media file cannot be read.");
        }
        catch (UnauthorizedAccessException)
        {
            return MediaValidationResult.Failure("Access to the selected media file was denied.");
        }

        return MediaValidationResult.Success;
    }
}
