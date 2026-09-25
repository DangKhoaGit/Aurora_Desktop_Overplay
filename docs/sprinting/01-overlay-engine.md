# Giai đoạn 1 — Overlay Engine

## Sprint 1: Cửa sổ overlay tĩnh

Mục tiêu: người dùng có thể mở một PNG/JPG, di chuyển và thay đổi kích thước overlay an toàn.

### Backlog

- Xây `OverlayItem`, value objects Position/Size và `IOverlayService`.
- Tạo `OverlayWindow` trong suốt, borderless, không taskbar/Alt+Tab; giữ đúng aspect ratio.
- Hỗ trợ PNG/JPG/BMP, opacity, show/hide, move, resize và reset position.
- Tạo Edit Mode với border/resize handles; Lock Mode không cho chỉnh nhầm.
- Xử lý lỗi decode, file bị xóa và ảnh vượt giới hạn cấu hình.
- Unit test model/service; smoke test DPI 100%, 150%, 200%.

### Tiêu chí nghiệm thu

- Overlay không lấy focus ngoài ý muốn, giữ aspect ratio và không hiện button taskbar.
- Move/resize/opacity phản hồi mượt; PNG tĩnh không tạo vòng render liên tục.
- File lỗi không làm crash app và đưa ra thông báo có thể hành động.

## Sprint 2: Win32 interaction và nhiều overlay

Mục tiêu: quản lý độc lập nhiều overlay với click-through và Z-order đúng.

### Backlog

- Xây `OverlayManager` và vòng đời create/remove/show/hide/update.
- Hỗ trợ nhiều overlay, duplicate, bring-to-front và Z-index.
- Bật/tắt click-through tức thời qua extended window styles.
- Hỗ trợ Normal/Always-on-top; thử nghiệm Desktop Layer và force-above-taskbar sau feature flag.
- Thêm thao tác khóa/mở khóa tất cả và chuyển Edit Mode không làm mất cấu hình click-through.
- Test với 1/5/10/20 overlay và hành vi focus/click trên app phía dưới.

### Tiêu chí nghiệm thu

- 20 PNG overlay có thể được tạo/xóa mà không leak window handle rõ rệt.
- Click-through thực sự chuyển click xuống cửa sổ bên dưới; Edit Mode khôi phục trạng thái trước đó khi thoát.
- Không gọi `SetWindowPos` mỗi frame; thay đổi Z-order theo sự kiện.

## Sprint 3: Media động và đa màn hình nền tảng

Mục tiêu: hỗ trợ GIF hiệu quả và đặt overlay chính xác trên nhiều monitor.

### Backlog

- Tách `ImageLoader`, `GifRenderer`, cache và cancellation/lifecycle.
- GIF: loop, pause, playback speed và FPS limit; chỉ render theo frame timing thực tế.
- Pause media khi hidden, opacity bằng 0, ngoài màn hình hoặc monitor sleep khi xác định được.
- Tạo `IMonitorService`; định danh monitor, work area, DPI và thay đổi topology.
- Khôi phục overlay bị off-screen về màn hình còn hoạt động.
- Benchmark 1/5/10 GIF; theo dõi CPU/GPU/RAM và object/handle leak.

### Tiêu chí nghiệm thu

- GIF chạy đúng timing cơ bản và dừng tiêu thụ render khi pause/hidden.
- Overlay giữ vị trí hợp lý khi đổi resolution/DPI, tháo hoặc gắn màn hình.
- Có artifact benchmark và danh sách giới hạn đã biết.

### Mốc cuối giai đoạn

Tạo pre-release `v0.1.0-alpha.1` để thử nội bộ; chưa quảng bá là MVP ổn định.
