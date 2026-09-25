# Sprint 2 — Nhật ký kiểm tra

Ngày thực hiện: 24/09/2026

## Đã triển khai

- Tận dụng `OverlayManager`, `OverlayWindow`, `WindowStyleService`, media loader và Control Center từ Sprint 0–1.
- Thêm `OverlayWindowCoordinator` quản lý ánh xạ model ↔ window và giải phóng window khi delete/close/app exit.
- Mở nhiều ảnh cùng lần chọn; Active Overlays list cho phép chọn và chỉnh từng overlay.
- Duplicate tạo ID độc lập, giữ media/size/state và offset vị trí 24 px.
- Delete, show/hide, lock/unlock, opacity, reset position và bring-to-front theo overlay.
- Normal/Always-on-top chuyển đổi runtime bằng `SetWindowPos` với `SWP_NOACTIVATE`.
- Click-through chuyển đổi runtime bằng `WS_EX_TRANSPARENT`.
- `WS_EX_TOOLWINDOW` và `WS_EX_NOACTIVATE` tiếp tục ngăn taskbar/Alt+Tab/focus ngoài ý muốn.
- Lock All và Edit Mode toàn cục; khi vào Edit Mode click-through được lưu rồi tắt, khi thoát được khôi phục.
- Không gọi `SetWindowPos` theo frame; chỉ gọi khi người dùng đổi Z-order/topmost.
- Feature flag Desktop Layer và Force Above Taskbar mặc định `false` trong `appsettings.json`.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode` — thành công.
- [x] `dotnet format --verify-no-changes --no-restore` — thành công.
- [x] `dotnet build -c Release --no-restore` — thành công, 0 warning, 0 error.
- [x] `dotnet test -c Release --no-build` — 12/12 test thành công.
- [x] Model test tạo, tra cứu và xóa độc lập 20 overlay.
- [x] Duplicate test xác nhận ID, tên, offset và kích thước độc lập.

## Cần kiểm tra thủ công trên desktop tương tác

- [ ] Mở đồng thời 1/5/10/20 overlay và xác minh từng item trong danh sách.
- [ ] Duplicate/delete không để lại cửa sổ hoặc window handle mồ côi.
- [ ] Click-through chuyển click xuống ứng dụng bên dưới.
- [ ] Edit Mode tạm tắt click-through và khôi phục đúng từng overlay khi thoát.
- [ ] Lock All ngăn move/resize trên toàn bộ overlay.
- [ ] Normal/Always-on-top và Bring to front đúng mà không lấy focus.
- [ ] Overlay không xuất hiện trong Alt+Tab hoặc taskbar.
- [ ] Theo dõi CPU/RAM/GDI/User handles trong vòng lặp tạo/xóa 20 overlay.

## Giới hạn và quyết định hoãn

- Desktop Layer và Force Above Taskbar chưa đưa ra UI; hai chế độ có feature flag tắt mặc định cho tới khi có ma trận test Windows 10/11 và Explorer restart.
- Thứ tự Z-index chưa được lưu lâu dài; persistence thuộc Sprint 4.
- Danh sách đang ưu tiên chức năng, chưa phải thiết kế Fluent hoàn chỉnh.
