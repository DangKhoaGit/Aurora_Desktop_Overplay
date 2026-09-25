using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Aurora.Desktop.Overlay.Core.Interfaces;
using Aurora.Desktop.Overlay.Core.Models;
using Aurora.Desktop.Overlay.Media;

namespace Aurora.Desktop.Overlay.Overlay;

public partial class OverlayWindow : Window
{
    private readonly IWindowStyleService _windowStyleService;
    private readonly IMonitorService _monitorService;
    private readonly LoadedImage _image;
    private readonly DispatcherTimer _animationTimer;
    private readonly double _aspectRatio;
    private bool _clickThrough;
    private bool _isLocked;
    private bool _isPaused;
    private double _playbackSpeed = 1.0;
    private int _fpsLimit = 30;
    private int _frameIndex;
    private HwndSource? _windowSource;
    private bool _constrainingLayout;

    public OverlayWindow(LoadedImage image, IWindowStyleService windowStyleService, IMonitorService monitorService)
    {
        ArgumentNullException.ThrowIfNull(image);
        _windowStyleService = windowStyleService;
        _monitorService = monitorService;
        _image = image;
        _aspectRatio = (double)image.PixelWidth / image.PixelHeight;
        _animationTimer = new DispatcherTimer(DispatcherPriority.Render);
        _animationTimer.Tick += AdvanceFrame;
        InitializeComponent();
        MediaImage.Source = image.Source;
        SourceInitialized += HandleSourceInitialized;
        LocationChanged += (_, _) =>
        {
            ConstrainToWorkArea(false);
            RaiseLayoutChanged();
            UpdatePlaybackState();
        };
        IsVisibleChanged += (_, _) => UpdatePlaybackState();
        Closed += (_, _) => StopAnimation();
        KeyDown += HandleKeyDown;
    }

    public event EventHandler<OverlayLayoutChangedEventArgs>? LayoutChanged;

    public bool IsLocked => _isLocked;

    public bool IsClickThrough => _clickThrough;

    public bool IsAnimated => _image.IsAnimated;

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        EditBorder.Visibility = locked ? Visibility.Collapsed : Visibility.Visible;
        ResizeThumb.Visibility = locked ? Visibility.Collapsed : Visibility.Visible;
    }

    public void SetOverlayOpacity(double opacity)
    {
        Opacity = Math.Clamp(opacity, 0.0, 1.0);
        UpdatePlaybackState();
    }

    public void SetPlayback(bool paused, double speed, int fpsLimit)
    {
        _isPaused = paused;
        _playbackSpeed = Math.Clamp(speed, 0.25, 4.0);
        _fpsLimit = Math.Clamp(fpsLimit, 1, 60);
        UpdatePlaybackState();
    }

    public void SetClickThrough(bool enabled)
    {
        _clickThrough = enabled;
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != nint.Zero) _windowStyleService.SetClickThrough(handle, enabled);
    }

    public void SetTopmost(bool enabled)
    {
        Topmost = enabled;
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != nint.Zero) _windowStyleService.SetTopmost(handle, enabled);
    }

    public void BringToFront(bool topmost)
    {
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != nint.Zero) _windowStyleService.BringToFront(handle, topmost);
    }

    public void ResetPosition()
    {
        var handle = new WindowInteropHelper(this).Handle;
        var monitor = _monitorService.GetNearestMonitor(handle);
        Left = monitor.WorkAreaPosition.X + Math.Max(0, (monitor.WorkAreaSize.Width - Width) / 2);
        Top = monitor.WorkAreaPosition.Y + Math.Max(0, (monitor.WorkAreaSize.Height - Height) / 2);
        RaiseLayoutChanged();
    }

    private void MoveOverlay(object sender, MouseButtonEventArgs args)
    {
        if (_isLocked || _clickThrough || args.LeftButton != MouseButtonState.Pressed ||
            ResizeThumb.IsMouseOver) return;
        DragMove();
    }

    private void ResizeOverlay(object sender, DragDeltaEventArgs args)
    {
        if (_isLocked) return;
        var proposedWidth = Math.Max(OverlaySize.MinimumDimension, Width + args.HorizontalChange);
        var proposedHeight = Math.Max(OverlaySize.MinimumDimension, Height + args.VerticalChange);
        if (Math.Abs(args.HorizontalChange) >= Math.Abs(args.VerticalChange))
            proposedHeight = proposedWidth / _aspectRatio;
        else
            proposedWidth = proposedHeight * _aspectRatio;

        var monitor = _monitorService.GetNearestMonitor(new WindowInteropHelper(this).Handle);
        var availableWidth = Math.Max(OverlaySize.MinimumDimension,
            monitor.WorkAreaPosition.X + monitor.WorkAreaSize.Width - Left);
        var availableHeight = Math.Max(OverlaySize.MinimumDimension,
            monitor.WorkAreaPosition.Y + monitor.WorkAreaSize.Height - Top);
        var scale = Math.Min(1.0, Math.Min(availableWidth / proposedWidth, availableHeight / proposedHeight));
        Width = Math.Max(OverlaySize.MinimumDimension, proposedWidth * scale);
        Height = Math.Max(OverlaySize.MinimumDimension, proposedHeight * scale);
        RaiseLayoutChanged();
    }

    private void HandleKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.F2)
        {
            _clickThrough = !_clickThrough;
            ApplyWindowStyles();
        }
        else if (args.Key == Key.Escape) Close();
    }

    private void ApplyWindowStyles()
    {
        var handle = new WindowInteropHelper(this).Handle;
        if (handle != nint.Zero) _windowStyleService.ConfigureOverlayWindow(handle, _clickThrough);
    }

    private void HandleSourceInitialized(object? sender, EventArgs args)
    {
        ApplyWindowStyles();
        _windowSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
        _windowSource?.AddHook(WindowProcedure);
        RestoreToVisibleArea();
        UpdatePlaybackState();
    }

    private nint WindowProcedure(nint windowHandle, int message, nint wordParameter,
        nint longParameter, ref bool handled)
    {
        const int displayChanged = 0x007E;
        const int dpiChanged = 0x02E0;
        if (message is displayChanged or dpiChanged)
        {
            Dispatcher.BeginInvoke(RestoreToVisibleArea, DispatcherPriority.Background);
        }
        return nint.Zero;
    }

    private void RestoreToVisibleArea()
    {
        ConstrainToWorkArea(true);
        RaiseLayoutChanged();
        UpdatePlaybackState();
    }

    private void ConstrainToWorkArea(bool constrainSize)
    {
        if (_constrainingLayout) return;
        var handle = new WindowInteropHelper(this).Handle;
        if (handle == nint.Zero) return;
        _constrainingLayout = true;
        try
        {
            if (constrainSize)
            {
                var monitor = _monitorService.GetNearestMonitor(handle);
                var scale = Math.Min(1.0, Math.Min(monitor.WorkAreaSize.Width / Width,
                    monitor.WorkAreaSize.Height / Height));
                if (scale < 1.0)
                {
                    Width *= scale;
                    Height *= scale;
                }
            }
            var position = _monitorService.EnsureVisible(new OverlayPosition(Left, Top),
                new OverlaySize(Width, Height), handle);
            Left = position.X;
            Top = position.Y;
        }
        finally
        {
            _constrainingLayout = false;
        }
    }

    private void AdvanceFrame(object? sender, EventArgs args)
    {
        if (!_image.IsAnimated) return;
        _frameIndex = (_frameIndex + 1) % _image.Frames.Count;
        MediaImage.Source = _image.Frames[_frameIndex];
        ScheduleCurrentFrame();
    }

    private void ScheduleCurrentFrame()
    {
        var sourceDelay = _image.FrameDurations[_frameIndex].TotalMilliseconds / _playbackSpeed;
        var fpsDelay = 1000.0 / _fpsLimit;
        _animationTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(sourceDelay, fpsDelay));
    }

    private void UpdatePlaybackState()
    {
        var shouldRun = _image.IsAnimated && !_isPaused && IsVisible && Opacity > 0 && IsOnScreen();
        if (!shouldRun)
        {
            _animationTimer.Stop();
            return;
        }
        ScheduleCurrentFrame();
        if (!_animationTimer.IsEnabled) _animationTimer.Start();
    }

    private bool IsOnScreen()
    {
        var right = Left + Width;
        var bottom = Top + Height;
        return _monitorService.GetMonitors().Any(monitor =>
            right > monitor.Position.X && Left < monitor.Position.X + monitor.Size.Width &&
            bottom > monitor.Position.Y && Top < monitor.Position.Y + monitor.Size.Height);
    }

    private void StopAnimation()
    {
        _animationTimer.Stop();
        _animationTimer.Tick -= AdvanceFrame;
        _windowSource?.RemoveHook(WindowProcedure);
    }

    private void RaiseLayoutChanged() => LayoutChanged?.Invoke(this,
        new OverlayLayoutChangedEventArgs(new OverlayPosition(Left, Top), new OverlaySize(Width, Height)));
}
