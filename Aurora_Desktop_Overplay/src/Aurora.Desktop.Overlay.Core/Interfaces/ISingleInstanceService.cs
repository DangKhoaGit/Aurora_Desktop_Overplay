namespace Aurora.Desktop.Overlay.Core.Interfaces;

public interface ISingleInstanceService : IDisposable
{
    event EventHandler? ActivationRequested;
    bool TryAcquire();
    void SignalExistingInstance();
    void StartListening();
}
