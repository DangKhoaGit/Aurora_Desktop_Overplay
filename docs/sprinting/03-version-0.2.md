# Giai đoạn 3 — Quản lý nội dung (`v0.2.0`)

## Sprint 7: Media Library và định dạng mở rộng

Mục tiêu: biến thao tác file rời rạc thành thư viện media có thể quản lý.

### Backlog

- Grid thumbnail, search, filter, sort, rename, remove, duplicate, favorite và tags.
- Metadata: loại, kích thước pixel, dung lượng, đường dẫn nội bộ và trạng thái file.
- Thumbnail generation/cache có invalidation và giới hạn dung lượng.
- Hỗ trợ WebP/APNG; quyết định SVG sau threat/performance review.
- Clipboard image/paste; deduplicate theo content hash khi phù hợp.
- Migration dữ liệu `v0.1` sang media repository mới.

### Tiêu chí nghiệm thu

- Thư viện 1.000 item vẫn tìm/filter/scroll đáp ứng ở cấu hình test chuẩn.
- Thumbnail lỗi không chặn toàn thư viện; xóa media đang dùng có cảnh báo và lựa chọn rõ ràng.
- Cấu hình `v0.1` được migrate mà không mất overlay.

## Sprint 8: Profiles, layout và performance mode

Mục tiêu: chuyển nhanh bộ overlay theo ngữ cảnh và giữ layout qua thay đổi màn hình.

### Backlog

- CRUD profile, switch profile và restore last profile.
- Snap theo mép/tâm/grid/overlay; anchor theo 9 vị trí với offset.
- Overlay groups: tạo, tách, move và show/hide cả nhóm.
- High Quality/Balanced/Low Power; FPS policy và smart render.
- Dashboard metric CPU/RAM ở tần suất thấp, không tự tạo overhead đáng kể.
- Regression position/profile trên nhiều resolution/DPI.

### Tiêu chí phát hành `v0.2.0`

- Switch profile không để lại orphan window và hoàn thành ổn định với 20 overlay.
- Anchor giữ vị trí tương đối sau đổi resolution/DPI; snap không gây rung/lặp update.
- Ba performance mode có hành vi đo được và được mô tả cho người dùng.
- Migration từ `v0.1.0`, installer upgrade và rollback dữ liệu đã được kiểm thử.
