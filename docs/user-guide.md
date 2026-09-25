# Aurora Desktop Overlay — Hướng dẫn sử dụng MVP

## Cài đặt và chạy

ADO hỗ trợ Windows 10/11 x64. Với bản installer, chạy file `*-setup.exe`; với bản portable, giải nén toàn bộ ZIP rồi chạy `AuroraDesktopOverlay.exe`.

Lần đầu Windows SmartScreen có thể cảnh báo nếu build chưa được ký số. Chỉ tiếp tục khi artifact và SHA-256 khớp với GitHub Release chính thức.

## Tạo và chỉnh overlay

1. Chọn **Add image**, kéo file vào Control Center hoặc dùng **Paste path**.
2. Chọn PNG, JPG/JPEG, BMP hoặc GIF. ADO copy file vào thư mục dữ liệu riêng.
3. Trong Edit Mode, kéo overlay để di chuyển và kéo handle màu xanh để resize giữ tỷ lệ.
4. Điều chỉnh opacity, always-on-top hoặc playback GIF trong panel Selected overlay.
5. Bật **Click-through** để chuột đi tới ứng dụng bên dưới; dùng Edit Mode để tạm tắt click-through khi cần chỉnh lại.
6. Dùng **Lock all** sau khi hoàn tất bố cục.

## Phím tắt toàn cục

| Phím | Hành động |
|---|---|
| `Ctrl+Shift+O` | Mở Control Center |
| `Ctrl+Shift+H` | Ẩn/hiện toàn bộ overlay |
| `Ctrl+Shift+L` | Khóa toàn bộ overlay |
| `Ctrl+Shift+E` | Bật/tắt Edit Mode |

## System tray và startup

Đóng hoặc minimize Control Center không thoát ứng dụng. Dùng biểu tượng ADO trong system tray để mở lại, ẩn/hiện overlay hoặc chọn **Exit**. Tùy chọn **Start with Windows** chạy ADO minimized sau 5 giây và khôi phục session trước.

## Dữ liệu người dùng

Dữ liệu được lưu tại `%LocalAppData%\AuroraDesktopOverlay`. Gỡ cài đặt không tự xóa media/config để tránh mất dữ liệu. Muốn xóa hoàn toàn, thoát ADO, sao lưu nếu cần rồi tự xóa thư mục này.
