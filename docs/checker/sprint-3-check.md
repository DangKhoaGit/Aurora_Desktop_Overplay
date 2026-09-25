# Sprint 3 — Nhật ký kiểm tra

Ngày thực hiện: 24/09/2026

## Đã triển khai

- Mở rộng loader ảnh tĩnh hiện có để giải mã GIF nhiều frame, đọc delay từ metadata và cache frame trong memory.
- Dispatcher timer theo delay thực tế của frame, có playback speed 0,5×/1×/2× và FPS limit 1–60.
- Loop animation; pause/resume riêng từng overlay.
- Smart pause khi overlay hidden, opacity 0, ngoài tất cả màn hình hoặc đã đóng.
- Timer/hook được tháo khi window đóng để tránh giữ window/media ngoài ý muốn.
- `IMonitorService` và Win32 implementation dùng `EnumDisplayMonitors`, `MonitorFromWindow`, `GetMonitorInfo`, `GetDpiForMonitor`.
- Lấy monitor bounds, work area, DPI và primary state; hiển thị tóm tắt monitor trong Control Center.
- Reset position dùng work area của monitor gần cửa sổ thay vì chỉ primary monitor.
- Xử lý `WM_DISPLAYCHANGE`/`WM_DPICHANGED`, đưa overlay off-screen trở lại monitor hợp lệ.
- Application manifest bật Per-Monitor V2 và tiếp tục chạy `asInvoker`.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode` — thành công.
- [x] `dotnet format --verify-no-changes --no-restore` — thành công.
- [x] `dotnet build -c Release --no-restore` — thành công, 0 warning, 0 error.
- [x] `dotnet test -c Release --no-build` — 16/16 test thành công.
- [x] GIF fixture hai frame được decode đúng số frame và delay 50/100 ms.
- [x] GIF corrupt tạo lỗi `InvalidDataException` có thể xử lý, không rò exception decoder ra UI.
- [x] Monitor integration test trả về display có bounds và DPI hợp lệ.

## Cần kiểm tra thủ công trên desktop tương tác

- [ ] GIF thực tế loop đúng, không nháy/lỗi disposal/transparency.
- [ ] Playback 0,5×/1×/2× và FPS limit phản hồi đúng.
- [ ] Pause, hidden và opacity 0 làm CPU render animation giảm về gần 0.
- [ ] Di chuyển hoàn toàn ra ngoài màn hình làm animation dừng.
- [ ] Rút/cắm monitor, đổi primary, resolution và scale không làm mất overlay.
- [ ] DPI 100%, 125%, 150%, 175%, 200% và hai monitor khác DPI.
- [ ] Sleep/wake monitor và Explorer restart.
- [ ] Theo dõi object, memory và USER/GDI handles sau nhiều lần tạo/xóa GIF.

## Benchmark cần ghi trên máy kiểm thử

| Kịch bản | CPU | GPU | RAM | USER/GDI handles | Ghi chú |
|---|---:|---:|---:|---:|---|
| Idle, không overlay | Chưa đo | Chưa đo | Chưa đo | Chưa đo | Desktop tương tác bắt buộc |
| 1 GIF | Chưa đo | Chưa đo | Chưa đo | Chưa đo | — |
| 5 GIF | Chưa đo | Chưa đo | Chưa đo | Chưa đo | — |
| 10 GIF | Chưa đo | Chưa đo | Chưa đo | Chưa đo | — |
| 10 GIF hidden | Chưa đo | Chưa đo | Chưa đo | Chưa đo | Kỳ vọng timer dừng |

## Giới hạn

- Frame GIF được cache đầy đủ để playback nhẹ CPU; loader giới hạn 80 triệu pixel/frame và 160 triệu decoded pixels tổng. Ngưỡng này cần hiệu chỉnh sau benchmark RAM thực tế.
- Theo dõi monitor dựa trên thông báo Windows, không polling liên tục.
- Benchmark CPU/GPU/RAM cần phiên desktop tương tác nên chưa tự động đánh dấu đạt.
