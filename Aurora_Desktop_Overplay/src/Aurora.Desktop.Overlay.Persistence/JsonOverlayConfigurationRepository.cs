using System.Text.Json;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Persistence;

public sealed class JsonOverlayConfigurationRepository(IApplicationPaths paths) : IOverlayConfigurationRepository, IDisposable
{
    private readonly SemaphoreSlim _saveGate = new(1, 1);

    public async Task<ConfigurationLoadResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(paths.ConfigurationFile))
            return new ConfigurationLoadResult(OverlayConfiguration.Create([]), false);

        try
        {
            return new ConfigurationLoadResult(
                await ReadAsync(paths.ConfigurationFile, cancellationToken), false);
        }
        catch (Exception primaryException) when (IsRecoverable(primaryException))
        {
            if (File.Exists(paths.ConfigurationBackupFile))
            {
                try
                {
                    var backup = await ReadAsync(paths.ConfigurationBackupFile, cancellationToken);
                    return new ConfigurationLoadResult(backup, true,
                        "The main configuration was invalid. Aurora restored the last backup.");
                }
                catch (Exception backupException) when (IsRecoverable(backupException))
                {
                    return EmptyWithWarning(primaryException, backupException);
                }
            }

            return EmptyWithWarning(primaryException);
        }
    }

    public async Task SaveAsync(OverlayConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        await _saveGate.WaitAsync(cancellationToken);
        try
        {
            Directory.CreateDirectory(paths.DataDirectory);
            Directory.CreateDirectory(paths.BackupDirectory);
            var temporaryFile = $"{paths.ConfigurationFile}.tmp";
            try
            {
                await using (var stream = new FileStream(temporaryFile, FileMode.Create, FileAccess.Write,
                    FileShare.None, 81920, FileOptions.Asynchronous | FileOptions.WriteThrough))
                {
                    await JsonSerializer.SerializeAsync(stream, configuration, JsonDefaults.Options, cancellationToken);
                    await stream.FlushAsync(cancellationToken);
                    stream.Flush(true);
                }

                if (File.Exists(paths.ConfigurationFile))
                    File.Replace(temporaryFile, paths.ConfigurationFile, paths.ConfigurationBackupFile, true);
                else
                    File.Move(temporaryFile, paths.ConfigurationFile);
            }
            finally
            {
                if (File.Exists(temporaryFile)) File.Delete(temporaryFile);
            }
        }
        finally
        {
            _saveGate.Release();
        }
    }

    private static async Task<OverlayConfiguration> ReadAsync(string path,
        CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var configuration = await JsonSerializer.DeserializeAsync<OverlayConfiguration>(
            stream, JsonDefaults.Options, cancellationToken)
            ?? throw new InvalidDataException("Configuration content was empty.");
        if (configuration.SchemaVersion != OverlayConfiguration.CurrentSchemaVersion)
            throw new InvalidDataException($"Unsupported configuration schema {configuration.SchemaVersion}.");
        if (configuration.Overlays is null)
            throw new InvalidDataException("Configuration overlay collection was missing.");
        return configuration;
    }

    private static ConfigurationLoadResult EmptyWithWarning(params Exception[] exceptions) =>
        new(OverlayConfiguration.Create([]), false,
            $"Configuration could not be loaded. A safe empty session was started. ({exceptions[0].GetType().Name})");

    private static bool IsRecoverable(Exception exception) =>
        exception is JsonException or IOException or UnauthorizedAccessException or NotSupportedException;

    public void Dispose() => _saveGate.Dispose();
}
