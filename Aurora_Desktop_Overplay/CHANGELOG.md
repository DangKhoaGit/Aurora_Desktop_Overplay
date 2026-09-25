# Changelog

## [Unreleased]

## [0.2.0] - 2026-09-25

### Added

- Giao diện Control Center mới với chế độ sáng và tối.
- Icon ứng dụng được dùng cho cửa sổ và system tray.
- Đặt tên overlay khi thêm ảnh và đổi tên sau khi thêm.

### Changed

- Cải thiện màu sắc, độ tương phản và trạng thái điều khiển trong dark mode.

### Fixed

- Giới hạn vị trí và kích thước overlay trong vùng làm việc của màn hình.
- Hộp thoại đổi tên tự điều chỉnh chiều cao ở các mức DPI khác nhau.

### Added

- Solution .NET 10/WPF theo module.
- Spike overlay trong suốt, topmost, không Alt+Tab và click-through.
- CI build/test trên Windows và kiểm thử nền tảng.
- Quản lý nhiều overlay với active list, duplicate, delete và bring-to-front.
- Trạng thái visibility, lock, click-through, always-on-top và opacity theo từng overlay.
- Edit Mode toàn cục khôi phục click-through sau khi thoát.
- GIF frame timing, playback speed, pause và FPS limit.
- Smart pause khi animation bị ẩn, opacity 0 hoặc nằm ngoài màn hình.
- Nhận diện monitor/work area/DPI, Per-Monitor V2 và phục hồi vị trí khi display thay đổi.
- Import media vào AppData qua picker, drag/drop hoặc clipboard path.
- JSON configuration schema, debounce autosave, atomic write, backup recovery và session restore.
- System tray, close/minimize-to-tray và explicit Exit cleanup.
- Global hotkeys, single-instance activation và startup cùng Windows có delay.
- Local daily file logging với size/retention rotation.
- Accessibility cơ bản, tài liệu người dùng và known limitations cho MVP.
- Self-contained portable/installer packaging và GitHub tag release workflow.
