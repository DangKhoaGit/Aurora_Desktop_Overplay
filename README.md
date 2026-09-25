# Aurora Desktop Overlay

[![CI](https://github.com/DangKhoaGit/Desktops-Overplay-Project/actions/workflows/ci.yml/badge.svg)](https://github.com/DangKhoaGit/Desktops-Overplay-Project/actions/workflows/ci.yml)
[![Release](https://img.shields.io/github/v/release/DangKhoaGit/Desktops-Overplay-Project?include_prereleases)](https://github.com/DangKhoaGit/Desktops-Overplay-Project/releases)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)
[![Platform: Windows](https://img.shields.io/badge/platform-Windows%2010%20%7C%2011-0078D4)](https://github.com/DangKhoaGit/Desktops-Overplay-Project/releases)

**Aurora Desktop Overlay (ADO)** là ứng dụng Windows mã nguồn mở giúp đặt ảnh và GIF trong suốt lên màn hình dưới dạng overlay. ADO phù hợp để trang trí desktop, hiển thị hình tham chiếu hoặc bố trí nội dung trực quan mà không can thiệp vào tiến trình của ứng dụng khác.

> Trạng thái hiện tại: `v0.1.0-rc.1` — bản release candidate dành cho kiểm thử trước khi phát hành stable.

## Điểm nổi bật

- Hiển thị PNG, JPG/JPEG, BMP và GIF dưới dạng overlay trong suốt.
- Tạo và quản lý nhiều overlay cùng lúc.
- Di chuyển, resize giữ tỷ lệ, điều chỉnh opacity và thứ tự hiển thị.
- Lock overlay, click-through và always-on-top.
- Edit Mode toàn cục để chỉnh lại các overlay đang click-through.
- Điều khiển GIF: pause, tốc độ phát và giới hạn FPS.
- Tự dừng animation khi overlay bị ẩn, opacity bằng 0 hoặc nằm ngoài màn hình.
- Hỗ trợ Per-Monitor V2 DPI và phục hồi vị trí khi cấu hình màn hình thay đổi.
- Tự lưu và khôi phục phiên làm việc.
- System tray, global hotkeys và tùy chọn khởi động cùng Windows.
- Không telemetry, không quảng cáo, không tài khoản và không cloud sync.

ADO không dùng DLL injection, process injection, đọc bộ nhớ game hoặc DirectX hook.

## Tải xuống

Tải phiên bản mới nhất tại [GitHub Releases](https://github.com/DangKhoaGit/Desktops-Overplay-Project/releases).

Mỗi release cung cấp:

- `*-setup.exe`: bộ cài đặt cho tài khoản Windows hiện tại.
- `*-portable.zip`: bản portable, giải nén trước khi chạy.
- `*.sha256`: checksum dùng để xác minh file tải xuống.

### Lưu ý về SmartScreen

ADO `v0.1.0` chưa được ký số nên Windows SmartScreen có thể hiển thị cảnh báo. Chỉ tải ứng dụng từ repository chính thức này và kiểm tra SHA-256 trước khi chạy.

Ví dụ kiểm tra installer bằng PowerShell:

```powershell
$artifact = ".\AuroraDesktopOverlay-0.1.0-rc.1-win-x64-setup.exe"
$expected = ((Get-Content "$artifact.sha256") -split "\s+")[0]
$actual = (Get-FileHash $artifact -Algorithm SHA256).Hash.ToLowerInvariant()

if ($actual -eq $expected) {
    "Checksum hợp lệ"
} else {
    "Checksum không khớp — không chạy file"
}
```

## Bắt đầu sử dụng

1. Chạy installer hoặc giải nén toàn bộ bản portable.
2. Mở `AuroraDesktopOverlay.exe`.
3. Chọn **Add image**, kéo thả file vào Control Center hoặc dùng **Paste path**.
4. Trong Edit Mode, kéo overlay để di chuyển và kéo handle để resize.
5. Điều chỉnh opacity, lock, click-through, always-on-top hoặc playback GIF.
6. Dùng **Lock all** sau khi hoàn thành bố cục.

Đóng hoặc thu nhỏ Control Center không thoát ADO. Chọn **Exit** từ biểu tượng system tray để đóng hoàn toàn ứng dụng.

## Phím tắt

| Phím | Hành động |
|---|---|
| `Ctrl+Shift+O` | Mở Control Center |
| `Ctrl+Shift+H` | Ẩn/hiện toàn bộ overlay |
| `Ctrl+Shift+L` | Khóa toàn bộ overlay |
| `Ctrl+Shift+E` | Bật/tắt Edit Mode |

## Dữ liệu và quyền riêng tư

Media, cấu hình, backup và log được lưu cục bộ tại:

```text
%LocalAppData%\AuroraDesktopOverlay
```

Gỡ cài đặt không tự xóa thư mục này để tránh làm mất media và bố cục của người dùng. Xem thêm [Privacy và Security](docs/privacy-security.md).

## Build từ source

### Yêu cầu

- Windows 10 hoặc Windows 11 x64.
- [.NET SDK 10.0.401](https://dotnet.microsoft.com/download) hoặc bản vá tương thích mới hơn.
- Inno Setup 6 nếu muốn build installer.

### Build và chạy test

```powershell
cd Aurora_Desktop_Overplay
dotnet restore --locked-mode
dotnet build -c Release --no-restore
dotnet test -c Release --no-build
dotnet run --project src/Aurora.Desktop.Overlay.App
```

### Tạo portable và installer

```powershell
cd Aurora_Desktop_Overplay
powershell -NoProfile -ExecutionPolicy Bypass -File .\build\Publish.ps1 -Version 0.1.0 -BuildInstaller
```

Artifact được tạo trong `Aurora_Desktop_Overplay/artifacts/release`.

## Cấu trúc dự án

```text
Aurora_Desktop_Overplay/
├── src/
│   ├── Aurora.Desktop.Overlay.App
│   ├── Aurora.Desktop.Overlay.Core
│   ├── Aurora.Desktop.Overlay.Infrastructure
│   ├── Aurora.Desktop.Overlay.Media
│   ├── Aurora.Desktop.Overlay.Overlay
│   ├── Aurora.Desktop.Overlay.Persistence
│   └── Aurora.Desktop.Overlay.Platform.Windows
├── tests/
└── build/
docs/
├── sprinting/
├── checker/
└── release/
```

## Tài liệu

- [Hướng dẫn sử dụng](docs/user-guide.md)
- [Khắc phục sự cố](docs/troubleshooting.md)
- [Privacy và Security](docs/privacy-security.md)
- [Các giới hạn đã biết](docs/known-limitations.md)
- [Release candidate checklist](docs/release/v0.1.0-rc.md)
- [Đặc tả dự án](docs/Desktop_Overlay_Project_Spec.md)
- [Lộ trình phát triển](docs/sprinting/README.md)

## Giới hạn hiện tại

- Chỉ hỗ trợ Windows 10/11 x64.
- Exclusive fullscreen, UAC Secure Desktop và một số game/anti-cheat có thể không hiển thị overlay.
- WebP, APNG, SVG và video chưa được hỗ trợ trong `v0.1.0`.
- Desktop Layer và Force Above Taskbar chưa được mở trong giao diện.
- Hotkey chưa thể tùy chỉnh từ UI.

Danh sách đầy đủ nằm trong [Known limitations](docs/known-limitations.md).

## Đóng góp và báo lỗi

Bạn có thể mở [GitHub Issue](https://github.com/DangKhoaGit/Desktops-Overplay-Project/issues) để báo lỗi hoặc đề xuất tính năng. Trước khi gửi log, hãy kiểm tra và xóa tên hoặc đường dẫn file riêng tư nếu cần.

Hướng dẫn đóng góp và quy trình báo cáo lỗ hổng nằm tại:

- [CONTRIBUTING.md](Aurora_Desktop_Overplay/CONTRIBUTING.md)
- [SECURITY.md](Aurora_Desktop_Overplay/SECURITY.md)

## Giấy phép

Aurora Desktop Overlay được phát hành theo [GNU General Public License v3.0](LICENSE). Khi phân phối binary hoặc phiên bản chỉnh sửa, bạn phải tuân thủ các điều khoản của GPL-3.0 và cung cấp source tương ứng.
