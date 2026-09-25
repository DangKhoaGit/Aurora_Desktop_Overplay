using Aurora.Desktop.Overlay.Core.Models;

namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface IGlobalHotkeyService : IDisposable
{
    event EventHandler<GlobalHotkeyAction>? Triggered;
    IReadOnlyList<string> RegisterDefaults(nint windowHandle);
}
