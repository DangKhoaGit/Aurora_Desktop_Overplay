using System.ComponentModel;
using System.Runtime.InteropServices;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Platform.Windows.Win32;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class WindowStyleService : IWindowStyleService
{
    public void ConfigureOverlayWindow(nint windowHandle, bool clickThrough)
    {
        var styles = GetStyles(windowHandle) | NativeMethods.WsExToolWindow | NativeMethods.WsExNoActivate;
        styles = clickThrough ? styles | NativeMethods.WsExTransparent : styles & ~NativeMethods.WsExTransparent;
        SetStyles(windowHandle, styles);
    }

    public void SetClickThrough(nint windowHandle, bool enabled)
    {
        var styles = GetStyles(windowHandle);
        styles = enabled ? styles | NativeMethods.WsExTransparent : styles & ~NativeMethods.WsExTransparent;
        SetStyles(windowHandle, styles);
    }

    public void SetTopmost(nint windowHandle, bool enabled) => SetZOrder(windowHandle,
        enabled ? NativeMethods.HwndTopmost : NativeMethods.HwndNoTopmost);

    public void BringToFront(nint windowHandle, bool topmost) => SetZOrder(windowHandle,
        topmost ? NativeMethods.HwndTopmost : NativeMethods.HwndTop);

    private static void SetZOrder(nint handle, nint insertAfter)
    {
        if (!NativeMethods.SetWindowPos(handle, insertAfter, 0, 0, 0, 0,
            NativeMethods.SwpNoMove | NativeMethods.SwpNoSize | NativeMethods.SwpNoActivate))
            throw new Win32Exception(Marshal.GetLastPInvokeError());
    }

    private static long GetStyles(nint handle)
    {
        Marshal.SetLastPInvokeError(0);
        var result = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GwlExStyle);
        ThrowIfFailed(result);
        return result.ToInt64();
    }

    private static void SetStyles(nint handle, long styles)
    {
        Marshal.SetLastPInvokeError(0);
        var result = NativeMethods.SetWindowLongPtr(handle, NativeMethods.GwlExStyle, new nint(styles));
        ThrowIfFailed(result);
    }

    private static void ThrowIfFailed(nint result)
    {
        var error = Marshal.GetLastPInvokeError();
        if (result == nint.Zero && error != 0) throw new Win32Exception(error);
    }
}
