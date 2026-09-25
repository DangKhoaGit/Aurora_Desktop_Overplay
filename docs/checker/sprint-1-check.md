# Sprint 1 — Nhật ký kiểm tra

Ngày thực hiện: 24/09/2026

## Đã triển khai

- Tên sản phẩm được áp dụng thống nhất trên UI: **Aurora Desktop Overlay (ADO)**.
- Tận dụng và mở rộng `OverlayItem`, `IOverlayService`, `OverlayManager`, `OverlayWindow`, Win32 style service và composition root từ Sprint 0.
- Thêm value object `OverlayPosition` và `OverlaySize`, kiểm tra tọa độ/kích thước và tính kích thước ban đầu giữ tỷ lệ ảnh.
- Overlay ảnh tĩnh PNG/JPG/JPEG/BMP: transparent, borderless, topmost, không taskbar/Alt+Tab.
- Edit Mode có viền và resize handle; resize giữ aspect ratio, kéo để di chuyển.
- Lock Mode ẩn edit chrome và ngăn move/resize ngoài ý muốn.
- Control Center hỗ trợ opacity 10–100%, hide/show, reset về giữa work area và hiển thị position/size.
- Validation file tồn tại, extension, file rỗng, giới hạn 50 MB; decoder cache ảnh vào memory và giới hạn 80 triệu pixel.
- Lỗi file/path/decode được chuyển thành thông báo người dùng thay vì làm đóng ứng dụng.
- State vị trí, kích thước, opacity, visibility và lock được cập nhật lại trong `OverlayManager`.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode` — thành công, lock file đã cập nhật.
- [x] `dotnet format --verify-no-changes --no-restore` — thành công.
- [x] `dotnet build -c Release --no-restore` — thành công, 0 warning, 0 error.
- [x] `dotnet test -c Release --no-build` — 10/10 test thành công (9 Core/Media và 1 Architecture).

## Cần kiểm tra thủ công trên desktop tương tác

- [ ] PNG ngang, PNG dọc, JPG và BMP hiển thị đúng.
- [ ] Kéo overlay không giật; resize giữ tỷ lệ và không nhỏ hơn 48 px.
- [ ] Lock ngăn move/resize; Edit mở lại viền và handle.
- [ ] Opacity, hide/show và reset position phản hồi đúng.
- [ ] Overlay không lấy focus ngoài ý muốn, không có taskbar button và không xuất hiện trong Alt+Tab.
- [ ] File corrupt/file bị xóa hiển thị thông báo và app tiếp tục chạy.
- [ ] Smoke test DPI 100%, 150%, 200% trên Windows 10 và Windows 11.
- [ ] Đo CPU idle/PNG tĩnh và kiểm tra không có render loop liên tục.

## Ghi chú phạm vi

- Control Center hiện điều khiển overlay mở gần nhất; danh sách và quản lý nhiều overlay đầy đủ thuộc Sprint 2.
- Click-through code từ spike vẫn được giữ để tái sử dụng, nhưng UX/hotkey hoàn chỉnh thuộc Sprint 2.
- Persistence chưa được nối vào editor; save/restore thuộc Sprint 4.
