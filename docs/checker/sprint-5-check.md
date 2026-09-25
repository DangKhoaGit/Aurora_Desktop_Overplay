# Sprint 5 — Nhật ký kiểm tra

Ngày thực hiện: 24/09/2026

## Đã triển khai

- System tray dùng Windows `NotifyIcon`: Open Control Center, Show All, Hide All, Edit Mode và Exit.
- Double-click tray icon mở Control Center.
- Đóng hoặc minimize Control Center chuyển app xuống tray; chỉ explicit Exit mới đóng engine.
- Exit thực hiện final atomic save, unregister hotkey, đóng overlay, ẩn tray icon và dispose host/services.
- Global hotkey qua `RegisterHotKey`/`UnregisterHotKey`, có `MOD_NOREPEAT`:
  - `Ctrl+Shift+O`: mở Control Center.
  - `Ctrl+Shift+H`: ẩn/hiện toàn bộ overlay.
  - `Ctrl+Shift+L`: khóa toàn bộ overlay.
  - `Ctrl+Shift+E`: bật/tắt Edit Mode.
- Conflict khi đăng ký hotkey được tổng hợp và hiển thị trong Control Center.
- Single instance bằng named mutex và named auto-reset event; lần mở thứ hai báo instance đầu đưa Control Center lên trước rồi thoát, không ghi đè config.
- Startup cùng Windows qua `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`, không cần quyền admin.
- Startup command được quote, chạy `--minimized --startup-delay=5`; delay bị giới hạn 0–60 giây.
- Startup checkbox phản ánh và thay đổi trạng thái registry.
- File logging local tại `logs/yyyy-MM-dd.log`, mức Information trở lên, tối đa 5 MB/file và giữ 7 file gần nhất.
- Lỗi ghi log không làm dừng ứng dụng; không có logging theo frame.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode` — thành công.
- [x] `dotnet format --verify-no-changes --no-restore` — thành công.
- [x] `dotnet build -c Release --no-restore` — thành công, 0 warning, 0 error.
- [x] Single-instance test xác minh instance thứ hai signal được instance đầu.
- [x] Startup command test xác minh quote executable và delay.
- [x] File logger test xác minh tạo daily log và ghi message.
- [x] Tổng kiểm thử: 24/24 thành công.

## Cần kiểm tra thủ công trên desktop tương tác

- [ ] Đóng nút X và minimize chỉ ẩn Control Center; overlay và tray vẫn chạy.
- [ ] Mỗi tray command và double-click thực hiện đúng.
- [ ] Exit xóa tray icon, đóng mọi overlay và process biến mất.
- [ ] Bốn global hotkey hoạt động khi Control Center ẩn và không tự lặp khi giữ phím.
- [ ] Mô phỏng hotkey conflict và xác minh cảnh báo nhưng app vẫn chạy.
- [ ] Mở executable lần hai đưa instance đầu lên trước, không tạo engine/window/config thứ hai.
- [ ] Bật/tắt startup, sign out/in Windows, xác minh delay 5 giây, minimized và restore session.
- [ ] Windows shutdown/restart không bị close-to-tray ngăn cản và config cuối được lưu.
- [ ] Log rotation 5 MB, giữ 7 file và không ghi liên tục theo frame.

## Giới hạn

- Hotkey đang dùng bộ mặc định; UI tùy biến tổ hợp phím sẽ được bổ sung khi hoàn thiện Settings.
- Tray đang dùng icon hệ thống mặc định; icon thương hiệu cần asset chính thức.
- Startup delay hiện mặc định 5 giây; chưa có ô chỉnh số giây trong UI.
