using System.Windows.Interop;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;
using Aurora.Desktop.Overlay.Platform.Windows.Win32;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class GlobalHotkeyService : IGlobalHotkeyService
{
    private const int HotkeyMessage = 0x0312;
    private readonly Dictionary<int, GlobalHotkeyAction> _registrations = [];
    private HwndSource? _source;
    private nint _windowHandle;

    public event EventHandler<GlobalHotkeyAction>? Triggered;

    public IReadOnlyList<string> RegisterDefaults(nint windowHandle)
    {
        DisposeRegistrations();
        _windowHandle = windowHandle;
        _source = HwndSource.FromHwnd(windowHandle)
            ?? throw new InvalidOperationException("The Control Center window source is unavailable.");
        _source.AddHook(WindowProcedure);

        var conflicts = new List<string>();
        Register(0xA001, GlobalHotkeyAction.OpenControlCenter, 0x4F, "Ctrl+Shift+O", conflicts);
        Register(0xA002, GlobalHotkeyAction.ToggleAllOverlays, 0x48, "Ctrl+Shift+H", conflicts);
        Register(0xA003, GlobalHotkeyAction.LockAllOverlays, 0x4C, "Ctrl+Shift+L", conflicts);
        Register(0xA004, GlobalHotkeyAction.ToggleEditMode, 0x45, "Ctrl+Shift+E", conflicts);
        return conflicts;
    }

    public void Dispose()
    {
        DisposeRegistrations();
        GC.SuppressFinalize(this);
    }

    private void Register(int id, GlobalHotkeyAction action, uint virtualKey, string display,
        List<string> conflicts)
    {
        var modifiers = NativeMethods.ModControl | NativeMethods.ModShift | NativeMethods.ModNoRepeat;
        if (NativeMethods.RegisterHotKey(_windowHandle, id, modifiers, virtualKey))
            _registrations.Add(id, action);
        else
            conflicts.Add(display);
    }

    private nint WindowProcedure(nint windowHandle, int message, nint wordParameter,
        nint longParameter, ref bool handled)
    {
        if (message == HotkeyMessage && _registrations.TryGetValue(wordParameter.ToInt32(), out var action))
        {
            handled = true;
            Triggered?.Invoke(this, action);
        }
        return nint.Zero;
    }

    private void DisposeRegistrations()
    {
        foreach (var id in _registrations.Keys) _ = NativeMethods.UnregisterHotKey(_windowHandle, id);
        _registrations.Clear();
        _source?.RemoveHook(WindowProcedure);
        _source = null;
        _windowHandle = nint.Zero;
    }
}
