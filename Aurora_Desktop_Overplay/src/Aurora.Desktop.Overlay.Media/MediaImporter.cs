using Aurora.Desktop.Overlay.Core.Interfaces;
using System.IO;

namespace Aurora.Desktop.Overlay.Media;

public sealed class MediaImporter(IApplicationPaths paths)
{
    public async Task<string> ImportAsync(string sourcePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        Directory.CreateDirectory(paths.MediaDirectory);
        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();
        var destination = Path.Combine(paths.MediaDirectory, $"{Guid.NewGuid():N}{extension}");

        await using var source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 81920, useAsync: true);
        await using var target = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            bufferSize: 81920, useAsync: true);
        try
        {
            await source.CopyToAsync(target, cancellationToken);
            await target.FlushAsync(cancellationToken);
            return destination;
        }
        catch
        {
            target.Close();
            if (File.Exists(destination)) File.Delete(destination);
            throw;
        }
    }
}
