namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface ITrayService : IDisposable
{
    event EventHandler? OpenRequested;
    event EventHandler? ShowAllRequested;
    event EventHandler? HideAllRequested;
    event EventHandler? EditModeRequested;
    event EventHandler? ExitRequested;
    void Initialize();
}
