using Aurora.Desktop.Overlay.Core.Interfaces;

namespace Aurora.Desktop.Overlay.Platform.Windows;

public sealed class SingleInstanceService(string instanceKey = "AuroraDesktopOverlay") : ISingleInstanceService
{
    private string MutexName { get; } = $@"Local\{instanceKey}.SingleInstance";
    private string ActivationEventName { get; } = $@"Local\{instanceKey}.Activate";
    private readonly EventWaitHandle _stopEvent = new(false, EventResetMode.ManualReset);
    private Mutex? _mutex;
    private EventWaitHandle? _activationEvent;
    private bool _ownsMutex;

    public event EventHandler? ActivationRequested;

    public bool TryAcquire()
    {
        _mutex = new Mutex(true, MutexName, out _ownsMutex);
        if (_ownsMutex)
            _activationEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ActivationEventName);
        return _ownsMutex;
    }

    public void SignalExistingInstance()
    {
        try
        {
            using var activationEvent = EventWaitHandle.OpenExisting(ActivationEventName);
            activationEvent.Set();
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            // The first process may still be starting; exiting remains safer than creating a second engine.
        }
    }

    public void StartListening()
    {
        if (!_ownsMutex || _activationEvent is null) return;
        _ = Task.Factory.StartNew(Listen, CancellationToken.None,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Dispose()
    {
        _stopEvent.Set();
        _activationEvent?.Dispose();
        if (_ownsMutex) _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        _stopEvent.Dispose();
        GC.SuppressFinalize(this);
    }

    private void Listen()
    {
        if (_activationEvent is null) return;
        var signaled = new WaitHandle[] { _activationEvent, _stopEvent };
        while (WaitHandle.WaitAny(signaled) == 0)
            ActivationRequested?.Invoke(this, EventArgs.Empty);
    }
}
