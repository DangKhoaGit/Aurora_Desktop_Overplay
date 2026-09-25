# Known limitations — MVP v0.1.0

- Chỉ hỗ trợ Windows 10/11 x64.
- Exclusive fullscreen, UAC Secure Desktop và một số game/anti-cheat có thể không hiển thị overlay.
- ADO không cam kết tương thích mọi game; không dùng injection/hook để ép tương thích.
- Desktop Layer và Force Above Taskbar chưa mở trong UI; taskbar/Start Menu khác nhau giữa các bản Windows.
- Định dạng MVP: PNG, JPG/JPEG, BMP và GIF. WebP/APNG/SVG/video chưa thuộc v0.1.0.
- Hotkey dùng tổ hợp mặc định, chưa tùy biến qua UI.
- Startup delay cố định 5 giây trong MVP.
- Xóa overlay chưa garbage-collect media do file có thể được nhiều overlay dùng chung.
- GIF cache frame để giảm CPU, có thể dùng nhiều RAM với animation lớn.
- Installer chưa ký số nếu dự án chưa có code-signing certificate, vì vậy SmartScreen có thể cảnh báo.
