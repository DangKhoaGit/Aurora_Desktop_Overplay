using System.Globalization;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aurora.Desktop.Overlay.Infrastructure;

public sealed class FileLoggerProvider(IApplicationPaths paths) : ILoggerProvider
{
    private const long MaximumLogSize = 5 * 1024 * 1024;
    private readonly Lock _sync = new();
    private bool _disposed;

    public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void Write(LogLevel level, string category, EventId eventId, string message, Exception? exception)
    {
        if (_disposed || level < LogLevel.Information) return;
        try
        {
            lock (_sync)
            {
                Directory.CreateDirectory(paths.LogDirectory);
                var logFile = Path.Combine(paths.LogDirectory,
                    $"{DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}.log");
                if (File.Exists(logFile) && new FileInfo(logFile).Length >= MaximumLogSize)
                {
                    var rolled = Path.Combine(paths.LogDirectory,
                        $"{DateTime.UtcNow.ToString("yyyy-MM-dd-HHmmss", CultureInfo.InvariantCulture)}.log");
                    File.Move(logFile, rolled, true);
                }

                var line = $"{DateTimeOffset.UtcNow:O} [{level}] {category} ({eventId.Id}) {message}";
                if (exception is not null) line += $" | {exception.GetType().Name}: {exception.Message}";
                File.AppendAllText(logFile, line + Environment.NewLine);
                DeleteExpiredLogs();
            }
        }
        catch (Exception writeException) when (writeException is IOException or UnauthorizedAccessException)
        {
            // Logging must never terminate the overlay process when the log directory is unavailable.
        }
    }

    private void DeleteExpiredLogs()
    {
        var expired = new DirectoryInfo(paths.LogDirectory).EnumerateFiles("*.log")
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .Skip(7);
        foreach (var file in expired) file.Delete();
    }

    private sealed class FileLogger(FileLoggerProvider provider, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel)) provider.Write(logLevel, category, eventId,
                formatter(state, exception), exception);
        }
    }
}
