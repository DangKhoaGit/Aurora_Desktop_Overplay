# Giai đoạn 2 — MVP hoàn chỉnh (`v0.1.0`)

## Sprint 4: Persistence và Control Center cơ bản

Mục tiêu: người dùng quản lý overlay bằng UI và không mất cấu hình sau khi đóng app.

### Backlog

- Dashboard, Active Overlay List và property editor cho position, size, opacity, lock, click-through, topmost, monitor, FPS.
- Import PNG/JPG/GIF qua file picker, drag/drop và paste path; copy media vào AppData theo chính sách rõ ràng.
- JSON repository có schema version, atomic write, debounce autosave và giá trị mặc định an toàn.
- Save/restore toàn bộ overlay; phát hiện config corrupt và fallback bản backup.
- Commands show/hide/duplicate/delete/lock/bring-to-front; xác nhận hành động mất dữ liệu.
- Integration test serialize/deserialize, migration version đầu và file I/O lỗi.

### Tiêu chí nghiệm thu

- Restart app khôi phục đúng overlay và thuộc tính; ghi file dang dở không phá cấu hình cũ.
- Luồng import → add overlay → chỉnh → tự lưu hoàn thành không quá 2–3 click cho thao tác phổ biến.

## Sprint 5: Tray, hotkey, startup và vòng đời app

Mục tiêu: app hoạt động như tiện ích Windows chạy nền đáng tin cậy.

### Backlog

- System tray menu: open, show/hide all, Edit Mode, settings, exit.
- Global hotkeys có thể cấu hình; phát hiện conflict và unregister khi thoát.
- Single instance; lần mở thứ hai đưa Control Center hiện tại lên trước.
- Close/minimize to tray; quy trình exit rõ ràng và cleanup windows/resources.
- Startup cùng Windows, start minimized, restore last session và startup delay.
- Logging có rotation; chỉ ghi lifecycle và lỗi quan trọng, không log mỗi frame.

### Tiêu chí nghiệm thu

- Hotkey/tray dùng được khi Control Center đóng; app thoát hoàn toàn bằng lệnh Exit.
- Bật/tắt startup phản ánh đúng trạng thái hệ thống và không tạo entry trùng.
- Mở app lần hai không tạo Overlay Engine thứ hai.

## Sprint 6: Hardening MVP và release candidate

Mục tiêu: đạt chất lượng phát hành công khai cho 15 chức năng MVP trong đặc tả.

### Backlog

- Hoàn thiện multi-monitor, DPI và xử lý monitor hot-plug.
- Functional regression cho import, add/delete, move/resize, opacity, lock, click-through, topmost, nhiều overlay.
- Ma trận Windows 10/11; 1/2 monitor; DPI 100/125/150/175/200%; sleep/wake và explorer restart.
- Performance soak test 8 giờ; test 1/5/10 GIF và 1/5/10/20 overlay.
- Accessibility cơ bản: keyboard navigation, focus indicator, contrast, tooltip cho tính năng khó hiểu.
- Hoàn thiện README, user guide, privacy/security statement, known limitations và troubleshooting.
- Đóng gói portable và installer; kiểm tra uninstall không xóa media/config người dùng nếu chưa xác nhận.

### Tiêu chí phát hành `v0.1.0`

- Không còn lỗi P0/P1; lỗi P2 còn lại có workaround và issue công khai.
- Idle CPU < 1%, PNG tĩnh gần 0% CPU và app chính nằm trong mục tiêu RAM 50–150 MB ở kịch bản chuẩn.
- Gói mới được cài/chạy/gỡ trên máy sạch Windows 10 và 11.
- Tất cả mục MVP ở phần 67 của đặc tả được demo và ghi nhận kết quả.
- GitHub release có binary, checksum, release notes và limitations về exclusive fullscreen/anti-cheat.
