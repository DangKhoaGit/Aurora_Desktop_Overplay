using System.Drawing;
using System.Windows.Forms;
using Aurora.Desktop.Overlay.Core.Interfaces;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class WindowsTrayService : ITrayService
{
    private NotifyIcon? _notifyIcon;

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

        _notifyIcon = new NotifyIcon
        {
            Text = "Aurora Desktop Overlay (ADO)",
            Icon = SystemIcons.Application,
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
        GC.SuppressFinalize(this);
    }
}
