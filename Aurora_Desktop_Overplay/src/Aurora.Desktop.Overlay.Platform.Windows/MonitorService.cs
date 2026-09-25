using System.ComponentModel;
using System.Runtime.InteropServices;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;
using Aurora.Desktop.Overlay.Platform.Windows.Win32;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class MonitorService : IMonitorService
{
    public IReadOnlyList<DisplayMonitor> GetMonitors()
    {
        var monitors = new List<DisplayMonitor>();
        var handle = GCHandle.Alloc(monitors);
        try
        {
            if (!NativeMethods.EnumDisplayMonitors(nint.Zero, nint.Zero, EnumerateMonitor,
                GCHandle.ToIntPtr(handle)))
                throw new Win32Exception(Marshal.GetLastPInvokeError());
        }
        finally
        {
            handle.Free();
        }
        return monitors;
    }

    public DisplayMonitor GetNearestMonitor(nint windowHandle)
    {
        var monitor = NativeMethods.MonitorFromWindow(windowHandle, NativeMethods.MonitorDefaultToNearest);
        if (monitor == nint.Zero) throw new Win32Exception(Marshal.GetLastPInvokeError());
        return ReadMonitor(monitor);
    }

    public OverlayPosition EnsureVisible(OverlayPosition position, OverlaySize size, nint windowHandle)
    {
        var monitor = GetNearestMonitor(windowHandle);
        var left = Math.Clamp(position.X, monitor.WorkAreaPosition.X,
            monitor.WorkAreaPosition.X + Math.Max(0, monitor.WorkAreaSize.Width - size.Width));
        var top = Math.Clamp(position.Y, monitor.WorkAreaPosition.Y,
            monitor.WorkAreaPosition.Y + Math.Max(0, monitor.WorkAreaSize.Height - size.Height));
        return new OverlayPosition(left, top);
    }

    private static bool EnumerateMonitor(nint monitor, nint deviceContext,
        ref NativeMethods.NativeRect bounds, nint data)
    {
        var monitors = (List<DisplayMonitor>)GCHandle.FromIntPtr(data).Target!;
        monitors.Add(ReadMonitor(monitor));
        return true;
    }

    private static DisplayMonitor ReadMonitor(nint monitor)
    {
        var info = new NativeMethods.MonitorInfo { Size = (uint)Marshal.SizeOf<NativeMethods.MonitorInfo>() };
        if (!NativeMethods.GetMonitorInfo(monitor, ref info))
            throw new Win32Exception(Marshal.GetLastPInvokeError());

        uint dpiX = 96;
        uint dpiY = 96;
        _ = NativeMethods.GetDpiForMonitor(monitor, 0, out dpiX, out dpiY);
        var monitorSize = new OverlaySize(info.Monitor.Right - info.Monitor.Left,
            info.Monitor.Bottom - info.Monitor.Top);
        var workSize = new OverlaySize(info.WorkArea.Right - info.WorkArea.Left,
            info.WorkArea.Bottom - info.WorkArea.Top);
        return new DisplayMonitor(
            $"monitor-{monitor.ToInt64():X}",
            new OverlayPosition(info.Monitor.Left, info.Monitor.Top),
            monitorSize,
            new OverlayPosition(info.WorkArea.Left, info.WorkArea.Top),
            workSize,
            dpiX,
            dpiY,
            (info.Flags & NativeMethods.MonitorInfoPrimary) != 0);
    }
}
