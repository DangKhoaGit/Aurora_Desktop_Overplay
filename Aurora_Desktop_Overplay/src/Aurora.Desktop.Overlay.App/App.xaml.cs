using System.Windows;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Overlay;
using Aurora.Desktop.Overlay.Platform.Windows;
using Aurora.Desktop.Overlay.Media;
using Aurora.Desktop.Overlay.Infrastructure;
using Aurora.Desktop.Overlay.Persistence;
using Aurora.Desktop.Overlay.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows.Interop;

namespace Aurora.Desktop.Overlay.App;

public partial class App : Application
{
    private static readonly Action<ILogger, Exception?> LogFinalSaveFailure =
        LoggerMessage.Define(LogLevel.Error, new EventId(1001, "FinalSaveFailure"),
            "The final overlay configuration could not be saved.");
    private IHost? _host;
    private bool _isPrimaryInstance;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var delayArgument = e.Args.FirstOrDefault(argument =>
            argument.StartsWith("--startup-delay=", StringComparison.OrdinalIgnoreCase));
        if (delayArgument is not null && int.TryParse(delayArgument.AsSpan(delayArgument.IndexOf('=') + 1), out var seconds))
            await Task.Delay(TimeSpan.FromSeconds(Math.Clamp(seconds, 0, 60)));

        _host = Host.CreateDefaultBuilder(e.Args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IWindowStyleService, WindowStyleService>();
                services.AddSingleton<IMonitorService, MonitorService>();
                services.AddSingleton<IGlobalHotkeyService, GlobalHotkeyService>();
                services.AddSingleton<ITrayService, WindowsTrayService>();
                services.AddSingleton<IStartupService, WindowsStartupService>();
                services.AddSingleton<ISingleInstanceService, SingleInstanceService>();
                services.AddSingleton<IOverlayService, OverlayManager>();
                services.AddSingleton<IApplicationPaths, ApplicationPaths>();
                services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
                services.AddSingleton<IOverlayConfigurationRepository, JsonOverlayConfigurationRepository>();
                services.AddSingleton<MediaFileValidator>();
                services.AddSingleton<StaticImageLoader>();
                services.AddSingleton<MediaImporter>();
                services.AddSingleton<OverlayWindowCoordinator>();
                services.AddSingleton<MainWindow>();
            })
            .ConfigureLogging(logging => logging.AddDebug())
            .Build();

        await _host.StartAsync();
        if (e.Args.Contains("--smoke-test", StringComparer.OrdinalIgnoreCase))
        {
            _ = _host.Services.GetRequiredService<IMonitorService>().GetMonitors();
            _ = await _host.Services.GetRequiredService<IOverlayConfigurationRepository>().LoadAsync();
            await _host.StopAsync(TimeSpan.FromSeconds(3));
            _host.Dispose();
            _host = null;
            Shutdown(0);
            return;
        }
        var singleInstance = _host.Services.GetRequiredService<ISingleInstanceService>();
        if (!singleInstance.TryAcquire())
        {
            singleInstance.SignalExistingInstance();
            await _host.StopAsync(TimeSpan.FromSeconds(3));
            _host.Dispose();
            _host = null;
            Shutdown();
            return;
        }
        _isPrimaryInstance = true;
        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
        WireWindowsLifecycle(MainWindow, singleInstance);
        if (e.Args.Contains("--minimized", StringComparer.OrdinalIgnoreCase)) MainWindow.Hide();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            if (_isPrimaryInstance)
            {
                try
                {
                    var coordinator = _host.Services.GetRequiredService<OverlayWindowCoordinator>();
                    var repository = _host.Services.GetRequiredService<IOverlayConfigurationRepository>();
                    await repository.SaveAsync(OverlayConfiguration.Create(coordinator.Items));
                }
                catch (Exception exception)
                {
                    LogFinalSaveFailure(_host.Services.GetRequiredService<ILogger<App>>(), exception);
                }
            }
            await _host.StopAsync(TimeSpan.FromSeconds(3));
            _host.Dispose();
        }
        base.OnExit(e);
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        (MainWindow as MainWindow)?.PrepareForSystemShutdown();
        base.OnSessionEnding(e);
    }

    private void WireWindowsLifecycle(Window mainWindow, ISingleInstanceService singleInstance)
    {
        var controlCenter = (MainWindow)mainWindow;
        var coordinator = _host!.Services.GetRequiredService<OverlayWindowCoordinator>();
        var tray = _host.Services.GetRequiredService<ITrayService>();
        tray.OpenRequested += (_, _) => controlCenter.ShowControlCenter();
        tray.ShowAllRequested += (_, _) => coordinator.ShowAll();
        tray.HideAllRequested += (_, _) => coordinator.HideAll();
        tray.EditModeRequested += (_, _) => controlCenter.ToggleEditMode();
        tray.ExitRequested += (_, _) => controlCenter.ExitApplication();
        tray.Initialize();

        singleInstance.ActivationRequested += (_, _) => Dispatcher.Invoke(controlCenter.ShowControlCenter);
        singleInstance.StartListening();

        var hotkeys = _host.Services.GetRequiredService<IGlobalHotkeyService>();
        hotkeys.Triggered += (_, action) => Dispatcher.Invoke(() => HandleHotkey(action, controlCenter, coordinator));
        var conflicts = hotkeys.RegisterDefaults(new WindowInteropHelper(controlCenter).Handle);
        controlCenter.ReportHotkeyConflicts(conflicts);
    }

    private static void HandleHotkey(GlobalHotkeyAction action, MainWindow window,
        OverlayWindowCoordinator coordinator)
    {
        switch (action)
        {
            case GlobalHotkeyAction.OpenControlCenter: window.ShowControlCenter(); break;
            case GlobalHotkeyAction.ToggleAllOverlays: coordinator.ToggleAll(); break;
            case GlobalHotkeyAction.LockAllOverlays: window.LockAllOverlays(); break;
            case GlobalHotkeyAction.ToggleEditMode: window.ToggleEditMode(); break;
            default: throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }
}
