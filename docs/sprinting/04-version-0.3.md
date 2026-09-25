# Giai đoạn 4 — Tự động hóa và widget (`v0.3.0`)

## Sprint 9: Scene và application rules

Mục tiêu: tự chuyển bố cục overlay dựa trên ứng dụng đang chạy mà không can thiệp process.

### Backlog

- Scene CRUD dựa trên profile nhưng có metadata/rules riêng.
- Theo dõi foreground/running process bằng API Windows công khai; không đọc memory hay inject.
- Rule theo process: show/hide overlay, đổi scene; priority và conflict resolution xác định được.
- Auto Scene có debounce/cooldown, manual override và nút tắt khẩn cấp.
- Focus rules, presentation/privacy mode ở phạm vi tối thiểu đã chốt.
- Audit log mức thông tin cho lần đổi scene/rule, không log nội dung riêng tư.

### Tiêu chí nghiệm thu

- Rule đổi scene đúng với app mẫu, không loop khi chuyển focus nhanh.
- App không yêu cầu quyền admin cho luồng thông thường và không truy cập memory process khác.
- Manual override thắng automation theo quy tắc được ghi trong UI/tài liệu.

## Sprint 10: Text, clock, timer và playlist

Mục tiêu: cung cấp widget thiết yếu và luân phiên media mà vẫn giữ renderer nhẹ.

### Backlog

- `IOverlayContent` chung cho media và widget.
- Text overlay: font, size, color, alignment, background/opacity cơ bản.
- Clock và timer: timezone/format, start/pause/reset, trạng thái được lưu.
- Playlist/random overlay: interval, loop, shuffle và xử lý item mất.
- Scheduler dùng event/tần suất phù hợp; không timer 1–10 ms toàn app.
- Accessibility và localization readiness cho chuỗi UI mới.

### Tiêu chí phát hành `v0.3.0`

- Clock/timer không drift đáng kể trong soak test và khôi phục đúng theo semantics đã định.
- Playlist không leak decoder/cache qua nhiều chu kỳ.
- Automation và widget không làm idle CPU vượt ngân sách chung.
- Upgrade từ `v0.2.0`, tài liệu rules/widget và release artifacts đã được kiểm thử.
