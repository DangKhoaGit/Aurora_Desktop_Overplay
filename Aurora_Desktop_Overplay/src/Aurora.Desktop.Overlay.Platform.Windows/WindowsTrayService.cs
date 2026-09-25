using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Aurora.Desktop.Overlay.Core.Interfaces;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class WindowsTrayService : ITrayService
{
    private NotifyIcon? _notifyIcon;
    private Icon? _applicationIcon;

    public event EventHandler? OpenRequested;
    public event EventHandler? ShowAllRequested;
    public event EventHandler? HideAllRequested;
    public event EventHandler? EditModeRequested;
    public event EventHandler? ExitRequested;

    public void Initialize()
    {
        if (_notifyIcon is not null) return;
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open Control Center", null, (_, _) => OpenRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("Show All Overlays", null, (_, _) => ShowAllRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("Hide All Overlays", null, (_, _) => HideAllRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add("Edit Mode", null, (_, _) => EditModeRequested?.Invoke(this, EventArgs.Empty));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty));

        _applicationIcon = LoadApplicationIcon();
        _notifyIcon = new NotifyIcon
        {
            Text = "Aurora Desktop Overlay (ADO)",
            Icon = _applicationIcon ?? SystemIcons.Application,
            ContextMenuStrip = menu,
            Visible = true
        };
        _notifyIcon.DoubleClick += (_, _) => OpenRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_notifyIcon is null) return;
        _notifyIcon.Visible = false;
        _notifyIcon.ContextMenuStrip?.Dispose();
        _notifyIcon.Dispose();
        _notifyIcon = null;
        _applicationIcon?.Dispose();
        _applicationIcon = null;
        GC.SuppressFinalize(this);
    }

    private static Icon? LoadApplicationIcon()
    {
        using var stream = typeof(WindowsTrayService).Assembly.GetManifestResourceStream(
            "Aurora.Desktop.Overlay.Platform.Windows.Assets.ADO_icon.png");
        if (stream is null) return null;
        using var source = new Bitmap(stream);
        using var trayBitmap = new Bitmap(source, new Size(32, 32));
        var handle = trayBitmap.GetHicon();
        try
        {
            using var icon = Icon.FromHandle(handle);
            return (Icon)icon.Clone();
        }
        finally
        {
            DestroyIcon(handle);
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyIcon(nint handle);
}
