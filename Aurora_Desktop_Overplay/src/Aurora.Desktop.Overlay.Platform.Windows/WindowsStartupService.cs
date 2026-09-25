using Aurora.Desktop.Overlay.Core.Interfaces;
using Microsoft.Win32;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class WindowsStartupService : IStartupService
{
    private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "AuroraDesktopOverlay";

    public bool IsEnabled
    {
        get
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryPath, false);
                return key?.GetValue(ValueName) is string;
            }
            catch (Exception exception) when (exception is UnauthorizedAccessException or System.Security.SecurityException)
            {
                return false;
            }
        }
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RegistryPath, true)
            ?? throw new InvalidOperationException("The Windows startup registry key could not be opened.");
        if (!enabled)
        {
            key.DeleteValue(ValueName, false);
            return;
        }

        var executable = Environment.ProcessPath
            ?? throw new InvalidOperationException("The application executable path is unavailable.");
        key.SetValue(ValueName, BuildCommand(executable, 5), RegistryValueKind.String);
    }

    public static string BuildCommand(string executable, int delaySeconds) =>
        $"\"{executable}\" --minimized --startup-delay={Math.Clamp(delaySeconds, 0, 60)}";
}
