namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface IWindowStyleService
{
    void ConfigureOverlayWindow(nint windowHandle, bool clickThrough);
    void SetClickThrough(nint windowHandle, bool enabled);
    void SetTopmost(nint windowHandle, bool enabled);
    void BringToFront(nint windowHandle, bool topmost);
}
