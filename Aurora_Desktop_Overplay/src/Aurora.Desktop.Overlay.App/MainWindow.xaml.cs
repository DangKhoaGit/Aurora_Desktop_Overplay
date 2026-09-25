using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using Aurora.Desktop.Overlay.Core.Models;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Media;
using Aurora.Desktop.Overlay.Overlay;
using Microsoft.Win32;

namespace Aurora.Desktop.Overlay.App;

public partial class MainWindow : Window
{
    private readonly OverlayWindowCoordinator _coordinator;
    private readonly MediaFileValidator _validator;
    private readonly StaticImageLoader _imageLoader;
    private readonly IMonitorService _monitorService;
    private readonly MediaImporter _mediaImporter;
    private readonly IOverlayConfigurationRepository _configurationRepository;
    private readonly IStartupService _startupService;
    private readonly ObservableCollection<OverlayItem> _items = [];
    private readonly DispatcherTimer _autosaveTimer = new() { Interval = TimeSpan.FromMilliseconds(750) };
    private bool _updatingControls;
    private bool _editMode;
    private bool _refreshPending;
    private bool _isRestoring = true;
    private bool _saveInProgress;
    private bool _allowClose;
    private bool _isDarkTheme;

    private static readonly IReadOnlyDictionary<string, (string Light, string Dark)> ThemeColors =
        new Dictionary<string, (string, string)>
        {
            ["WindowBrush"] = ("#FFF7F8FC", "#FF05070B"),
            ["SurfaceBrush"] = ("#FFFFFFFF", "#FF090D14"),
            ["SurfaceMutedBrush"] = ("#FFF0F2F8", "#FF0D1420"),
            ["TextBrush"] = ("#FF172033", "#FF62C7FF"),
            ["MutedTextBrush"] = ("#FF667085", "#FF70A9D1"),
            ["BorderBrush"] = ("#FFDCE1EC", "#FF245D8A"),
            ["AccentBrush"] = ("#FF6C5CE7", "#FF168DCC"),
            ["AccentSoftBrush"] = ("#FFEDEAFF", "#FF071D2D"),
            ["PrimaryTextBrush"] = ("#FFFFFFFF", "#FF02070A"),
            ["SelectionBrush"] = ("#FFDCD6FF", "#FF103E5C")
        };

    public MainWindow(OverlayWindowCoordinator coordinator, MediaFileValidator validator,
        StaticImageLoader imageLoader, IMonitorService monitorService, MediaImporter mediaImporter,
        IOverlayConfigurationRepository configurationRepository, IStartupService startupService)
    {
        _coordinator = coordinator;
        _validator = validator;
        _imageLoader = imageLoader;
        _monitorService = monitorService;
        _mediaImporter = mediaImporter;
        _configurationRepository = configurationRepository;
        _startupService = startupService;
        InitializeComponent();
        OverlayList.ItemsSource = _items;
        _coordinator.Changed += CoordinatorChanged;
        _autosaveTimer.Tick += AutosaveTick;
        Loaded += RestoreSession;
        StateChanged += (_, _) =>
        {
            if (WindowState == WindowState.Minimized) Hide();
        };
        StartupCheckBox.IsChecked = _startupService.IsEnabled;
        SetEditorEnabled(false);
    }

    private OverlayItem? SelectedItem => OverlayList.SelectedItem as OverlayItem;

    private void ToggleThemeClick(object sender, RoutedEventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;
        foreach (var (key, colors) in ThemeColors)
            Application.Current.Resources[key] = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(
                    _isDarkTheme ? colors.Dark : colors.Light));
        ApplySystemControlColors();
        ApplyTitleBarTheme();
        ThemeButton.Content = _isDarkTheme ? "☀  Light mode" : "☾  Dark mode";
        StatusText.Text = _isDarkTheme ? "Dark mode enabled." : "Light mode enabled.";
    }

    private static void ApplySystemControlColors()
    {
        var resources = Application.Current.Resources;
        resources[SystemColors.WindowBrushKey] = resources["SurfaceBrush"];
        resources[SystemColors.ControlBrushKey] = resources["SurfaceBrush"];
        resources[SystemColors.ControlLightBrushKey] = resources["SurfaceMutedBrush"];
        resources[SystemColors.ControlTextBrushKey] = resources["TextBrush"];
        resources[SystemColors.WindowTextBrushKey] = resources["TextBrush"];
        resources[SystemColors.HighlightBrushKey] = resources["SelectionBrush"];
        resources[SystemColors.HighlightTextBrushKey] = resources["TextBrush"];
        resources[SystemColors.GrayTextBrushKey] = resources["MutedTextBrush"];
    }

    private void ApplyTitleBarTheme()
    {
        var enabled = _isDarkTheme ? 1 : 0;
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != nint.Zero)
            _ = DwmSetWindowAttribute(handle, 20, ref enabled, sizeof(int));
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(nint windowHandle, int attribute,
        ref int attributeValue, int attributeSize);

    private async void OpenOverlayClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select an image for Aurora Desktop Overlay",
            Filter = "Supported images|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*",
            Multiselect = true
        };
        if (dialog.ShowDialog(this) != true) return;

        await ImportPathsAsync(dialog.FileNames);
    }

    private async void PastePathClick(object sender, RoutedEventArgs e)
    {
        var paths = Clipboard.ContainsFileDropList()
            ? Clipboard.GetFileDropList().Cast<string>()
            : Clipboard.ContainsText()
                ? Clipboard.GetText().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : [];
        await ImportPathsAsync(paths);
    }

    private async void WindowDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            await ImportPathsAsync(paths);
    }

    private async Task ImportPathsAsync(IEnumerable<string> paths)
    {
        OverlayItem? lastCreated = null;
        var importedCount = 0;
        foreach (var path in paths)
        {
            var normalizedPath = path.Trim().Trim('"');
            var validation = _validator.Validate(normalizedPath);
            if (!validation.IsValid)
            {
                ShowError($"{Path.GetFileName(normalizedPath)}: {validation.ErrorMessage}");
                continue;
            }

            try
            {
                var defaultName = Path.GetFileNameWithoutExtension(normalizedPath);
                var nameDialog = new OverlayNameDialog(defaultName, "Name your overlay") { Owner = this };
                if (nameDialog.ShowDialog() != true) continue;
                var importedPath = await _mediaImporter.ImportAsync(normalizedPath);
                lastCreated = _coordinator.Create(importedPath, _imageLoader.Load(importedPath), nameDialog.OverlayName);
                importedCount++;
            }
            catch (InvalidDataException exception)
            {
                ShowError($"{Path.GetFileName(normalizedPath)}: {exception.Message}");
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                ShowError($"{Path.GetFileName(normalizedPath)} could not be opened.");
            }
        }

        RefreshOverlayList(lastCreated?.Id);
        if (lastCreated is not null) StatusText.Text = $"Imported {importedCount} media file(s) into AppData.";
    }

    private void DuplicateClick(object sender, RoutedEventArgs e)
    {
        if (SelectedItem is not { } selected) return;
        var copy = _coordinator.Duplicate(selected.Id);
        RefreshOverlayList(copy?.Id);
        StatusText.Text = copy is null ? "Overlay could not be duplicated." : $"Duplicated {selected.Name}.";
    }

    private void RenameClick(object sender, RoutedEventArgs e)
    {
        if (SelectedItem is not { } selected) return;
        var dialog = new OverlayNameDialog(selected.Name, "Rename overlay") { Owner = this };
        if (dialog.ShowDialog() != true) return;
        if (_coordinator.Rename(selected.Id, dialog.OverlayName))
        {
            RefreshOverlayList(selected.Id);
            StatusText.Text = $"Renamed overlay to {dialog.OverlayName}.";
        }
    }

    private void DeleteClick(object sender, RoutedEventArgs e)
    {
        if (SelectedItem is not { } selected) return;
        _coordinator.Remove(selected.Id);
        RefreshOverlayList();
        StatusText.Text = $"Deleted {selected.Name}.";
    }

    private void BringToFrontClick(object sender, RoutedEventArgs e)
    {
        if (SelectedItem is not { } selected) return;
        _coordinator.BringToFront(selected.Id);
        StatusText.Text = $"Brought {selected.Name} to front without activating it.";
    }

    private void LockAllClick(object sender, RoutedEventArgs e)
    {
        if (_editMode) _coordinator.SetEditMode(false);
        else _coordinator.LockAll();
        _editMode = false;
        EditModeButton.Content = "Edit mode";
        StatusText.Text = "All overlays locked.";
    }

    private void ToggleEditModeClick(object sender, RoutedEventArgs e)
        => ToggleEditMode();

    public void ToggleEditMode()
    {
        _editMode = !_editMode;
        _coordinator.SetEditMode(_editMode);
        EditModeButton.Content = _editMode ? "Exit edit" : "Edit mode";
        StatusText.Text = _editMode
            ? "Edit mode enabled. Click-through is temporarily disabled."
            : "Edit mode closed. Previous click-through states restored.";
    }

    public void LockAllOverlays() => LockAllClick(this, new RoutedEventArgs());

    public void ShowControlCenter()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    public void ExitApplication()
    {
        _allowClose = true;
        Close();
    }

    public void PrepareForSystemShutdown() => _allowClose = true;

    public void ReportHotkeyConflicts(IReadOnlyCollection<string> conflicts)
    {
        if (conflicts.Count > 0)
            StatusText.Text = $"Global hotkey conflict: {string.Join(", ", conflicts)}. Change the other application shortcut and restart ADO.";
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_allowClose)
        {
            e.Cancel = true;
            Hide();
            StatusText.Text = "ADO is still running in the system tray.";
        }
        base.OnClosing(e);
    }

    private void StartupClick(object sender, RoutedEventArgs e)
    {
        try
        {
            _startupService.SetEnabled(StartupCheckBox.IsChecked == true);
            StatusText.Text = StartupCheckBox.IsChecked == true
                ? "ADO will start minimized with Windows."
                : "Windows startup disabled.";
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or InvalidOperationException)
        {
            StartupCheckBox.IsChecked = _startupService.IsEnabled;
            ShowError($"Windows startup could not be changed: {exception.Message}");
        }
    }

    private void OverlaySelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateEditor();

    private void VisibleClick(object sender, RoutedEventArgs e) => ApplySelected(
        item => _coordinator.SetVisible(item.Id, VisibleCheckBox.IsChecked == true));

    private void LockedClick(object sender, RoutedEventArgs e) => ApplySelected(
        item => _coordinator.SetLocked(item.Id, LockedCheckBox.IsChecked == true));

    private void ClickThroughClick(object sender, RoutedEventArgs e) => ApplySelected(
        item => _coordinator.SetClickThrough(item.Id, ClickThroughCheckBox.IsChecked == true));

    private void TopmostClick(object sender, RoutedEventArgs e) => ApplySelected(
        item => _coordinator.SetTopmost(item.Id, TopmostCheckBox.IsChecked == true));

    private void OpacityChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_updatingControls && IsLoaded && SelectedItem is { } item)
            _coordinator.SetOpacity(item.Id, e.NewValue);
    }

    private void PlaybackChanged(object sender, RoutedEventArgs e) => ApplyPlayback();

    private void PlaybackSelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyPlayback();

    private void FpsChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => ApplyPlayback();

    private void ApplyPlayback()
    {
        if (_updatingControls || !IsLoaded || SelectedItem is not { } item) return;
        var speed = double.TryParse(PlaybackSpeedComboBox.SelectedValue?.ToString(),
            NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : 1.0;
        _coordinator.SetPlayback(item.Id, PausedCheckBox.IsChecked == true, speed, (int)FpsSlider.Value);
    }

    private void ResetPositionClick(object sender, RoutedEventArgs e)
    {
        if (SelectedItem is { } item) _coordinator.ResetPosition(item.Id);
    }

    private void ApplySelected(Action<OverlayItem> action)
    {
        if (!_updatingControls && SelectedItem is { } item) action(item);
    }

    private void CoordinatorChanged(object? sender, EventArgs e)
    {
        if (!_isRestoring)
        {
            _autosaveTimer.Stop();
            _autosaveTimer.Start();
        }
        if (_refreshPending) return;
        _refreshPending = true;
        Dispatcher.BeginInvoke(() =>
        {
            _refreshPending = false;
            RefreshOverlayList(SelectedItem?.Id);
        }, DispatcherPriority.Background);
    }

    private async void RestoreSession(object sender, RoutedEventArgs e)
    {
        Loaded -= RestoreSession;
        var restored = 0;
        var skipped = 0;
        try
        {
            var result = await _configurationRepository.LoadAsync();
            foreach (var item in result.Configuration.Overlays)
            {
                var validation = _validator.Validate(item.MediaPath);
                if (!validation.IsValid)
                {
                    skipped++;
                    continue;
                }

                try
                {
                    if (_coordinator.Restore(item, _imageLoader.Load(item.MediaPath))) restored++;
                }
                catch (Exception exception) when (exception is InvalidDataException or IOException or UnauthorizedAccessException or ArgumentException)
                {
                    skipped++;
                }
            }

            StatusText.Text = result.Warning ?? $"Restored {restored} overlay(s).";
            if (skipped > 0) StatusText.Text += $" Skipped {skipped} invalid or missing media item(s).";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            ShowError($"Configuration could not be restored: {exception.Message}");
        }
        finally
        {
            _isRestoring = false;
            RefreshOverlayList();
        }
    }

    private async void AutosaveTick(object? sender, EventArgs e)
    {
        _autosaveTimer.Stop();
        if (_saveInProgress || _isRestoring) return;
        _saveInProgress = true;
        try
        {
            await _configurationRepository.SaveAsync(OverlayConfiguration.Create(_coordinator.Items));
            StatusText.Text = $"Configuration saved automatically at {DateTime.Now:t}.";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            StatusText.Text = $"Autosave failed: {exception.Message}";
        }
        finally
        {
            _saveInProgress = false;
        }
    }

    private void RefreshOverlayList(Guid? preferredId = null)
    {
        preferredId ??= SelectedItem?.Id;
        _items.Clear();
        foreach (var item in _coordinator.Items.OrderBy(item => item.Name)) _items.Add(item);
        OverlayList.SelectedItem = preferredId is Guid id ? _items.FirstOrDefault(item => item.Id == id) : _items.FirstOrDefault();
        UpdateEditor();
    }

    private void UpdateEditor()
    {
        _updatingControls = true;
        var item = SelectedItem;
        SetEditorEnabled(item is not null);
        if (item is not null)
        {
            VisibleCheckBox.IsChecked = item.IsVisible;
            LockedCheckBox.IsChecked = item.IsLocked;
            ClickThroughCheckBox.IsChecked = item.IsClickThrough;
            TopmostCheckBox.IsChecked = item.IsTopmost;
            OpacitySlider.Value = item.Opacity;
            PausedCheckBox.IsChecked = item.IsPaused;
            PlaybackSpeedComboBox.SelectedValue = item.PlaybackSpeed.ToString("0.##",
                CultureInfo.InvariantCulture);
            FpsSlider.Value = item.FpsLimit;
            var isAnimated = _coordinator.IsAnimated(item.Id);
            PausedCheckBox.IsEnabled = isAnimated;
            PlaybackSpeedComboBox.IsEnabled = isAnimated;
            FpsSlider.IsEnabled = isAnimated;
            var monitors = _monitorService.GetMonitors();
            MonitorText.Text = $"Displays detected: {monitors.Count} — " +
                string.Join(", ", monitors.Select(monitor => $"{monitor.WorkAreaSize.Width:0}×{monitor.WorkAreaSize.Height:0} @ {monitor.DpiX} DPI"));
            DetailsText.Text = $"{item.Name} — {item.Size.Width:0} × {item.Size.Height:0} px\n" +
                $"X {item.Position.X:0}, Y {item.Position.Y:0} — opacity {item.Opacity:P0}";
        }
        else DetailsText.Text = "No overlay selected.";
        _updatingControls = false;
    }

    private void SetEditorEnabled(bool enabled)
    {
        VisibleCheckBox.IsEnabled = enabled;
        LockedCheckBox.IsEnabled = enabled;
        ClickThroughCheckBox.IsEnabled = enabled;
        TopmostCheckBox.IsEnabled = enabled;
        OpacitySlider.IsEnabled = enabled;
        PausedCheckBox.IsEnabled = enabled;
        PlaybackSpeedComboBox.IsEnabled = enabled;
        FpsSlider.IsEnabled = enabled;
    }

    private void ShowError(string message)
    {
        StatusText.Text = message;
        MessageBox.Show(this, message, "Aurora Desktop Overlay", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
