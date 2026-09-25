# Sprint 4 — Nhật ký kiểm tra

Ngày thực hiện: 24/09/2026

## Đã triển khai

- Tận dụng toàn bộ `OverlayItem`, `OverlayManager` và `OverlayWindowCoordinator` để serialize/restore cùng ID và trạng thái hiện có.
- `OverlayConfiguration` schema version 1 chứa timestamp và danh sách overlay.
- `JsonOverlayConfigurationRepository` đọc/ghi bất đồng bộ bằng `System.Text.Json`.
- Atomic write: serialize vào `config.json.tmp`, flush-to-disk, sau đó `File.Replace` và giữ `backups/config.backup.json`.
- Semaphore bảo vệ các lần autosave/final-save đồng thời dùng chung temp file.
- Khi config chính corrupt/không đọc được, tự thử backup; nếu cả hai lỗi thì mở safe empty session với cảnh báo.
- Debounce autosave 750 ms sau thay đổi model; ghi lần cuối trước khi host dừng.
- Khôi phục vị trí, kích thước, opacity, visible, lock, click-through, topmost và playback khi mở app.
- Media thiếu/corrupt trong lúc restore được bỏ qua riêng lẻ, không chặn phần session còn lại.
- Import PNG/JPG/JPEG/BMP/GIF qua file picker, multi-select, drag/drop, clipboard file list hoặc đường dẫn text.
- Media được copy vào `%LocalAppData%\AuroraDesktopOverlay\media` với tên GUID; app không phụ thuộc file nguồn sau import.

## Cấu trúc dữ liệu hiện tại

```text
%LocalAppData%\AuroraDesktopOverlay
├── config.json
├── media\
│   └── <guid>.<extension>
└── backups\
    └── config.backup.json
```

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode` — thành công.
- [x] `dotnet format --verify-no-changes --no-restore` — thành công.
- [x] `dotnet build -c Release --no-restore` — thành công, 0 warning, 0 error.
- [x] JSON round-trip giữ nguyên `OverlayItem`.
- [x] Config chính corrupt phục hồi đúng bản atomic backup trước đó.
- [x] Media importer copy đúng bytes vào managed media directory.
- [x] Nhiều save đồng thời được tuần tự hóa và file cuối vẫn load được.
- [x] Tổng kiểm thử: 21/21 thành công.

## Cần kiểm tra thủ công trên desktop tương tác

- [ ] Import bằng picker, multi-select, drag/drop, clipboard file list và clipboard text path.
- [ ] Xóa/đổi tên file nguồn sau import không ảnh hưởng overlay.
- [ ] Restart app khôi phục đúng mọi thuộc tính và GIF playback.
- [ ] Kéo/resize liên tục chỉ tạo lần save cuối sau debounce, UI không giật.
- [ ] Mô phỏng disk full/read-only/access denied và xác minh UI còn chạy.
- [ ] Kill process giữa lúc save rồi mở lại; config chính hoặc backup vẫn phục hồi được.
- [ ] Config chứa media bị xóa/corrupt: item đó bị skip, item khác vẫn restore.

## Giới hạn và việc để lại

- Xóa overlay chưa tự xóa media được quản lý vì một file có thể đang được duplicate sử dụng; garbage collection media sẽ làm cùng Media Library ở Sprint 7.
- Schema hiện là version 1; migration từ schema cũ sẽ được thêm khi có thay đổi schema đầu tiên.
- UI hiện thông báo restore tổng hợp, chưa có màn hình chi tiết từng item bị bỏ qua.
