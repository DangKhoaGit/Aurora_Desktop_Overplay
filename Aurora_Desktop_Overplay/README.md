# Aurora Desktop Overlay

Ứng dụng Windows hiển thị nội dung dưới dạng overlay trong suốt, nhẹ và không can thiệp vào process khác.

## Yêu cầu và lệnh

- Windows 10/11 x64; .NET SDK 10.0.401 hoặc bản vá mới hơn.

```powershell
dotnet restore
dotnet build -c Release --no-restore
dotnet test -c Release --no-build
dotnet run --project src/Aurora.Desktop.Overlay.App
```

Trong bản Sprint 1, chọn **Open image** để tạo overlay. Control Center cho phép khóa/chỉnh sửa, ẩn/hiện, đổi opacity và đưa overlay về giữa màn hình; kéo nút màu xanh để resize đúng tỷ lệ.

Sprint 2 bổ sung danh sách nhiều overlay, duplicate/delete, bring-to-front, click-through, always-on-top, Lock All và Edit Mode toàn cục. Desktop Layer và force-above-taskbar đang bị tắt bằng feature flag vì cần thêm ma trận kiểm thử Windows.

Sprint 3 hỗ trợ GIF theo timing từng frame, playback speed, pause và FPS limit. Animation tự ngừng khi hidden, opacity 0 hoặc ngoài màn hình. Ứng dụng bật Per-Monitor V2 và tự đưa overlay trở lại work area khi topology/DPI thay đổi.

Sprint 4 copy media được import vào `%LocalAppData%\AuroraDesktopOverlay\media`, tự lưu cấu hình sau 750 ms và khôi phục session khi mở lại. Có thể import bằng file picker, kéo thả hoặc paste đường dẫn/file từ clipboard. Cấu hình được ghi atomic và có backup fallback.

Sprint 5 chạy ADO như tiện ích system tray. Đóng/minimize Control Center không tắt overlay; dùng **Exit** trong tray để thoát hoàn toàn. Hotkeys mặc định: `Ctrl+Shift+O` mở Control Center, `Ctrl+Shift+H` ẩn/hiện tất cả, `Ctrl+Shift+L` khóa tất cả và `Ctrl+Shift+E` bật/tắt Edit Mode. Startup cùng Windows chạy minimized sau 5 giây.

## Tài liệu MVP

- [Hướng dẫn sử dụng](../docs/user-guide.md)
- [Troubleshooting](../docs/troubleshooting.md)
- [Privacy và Security](../docs/privacy-security.md)
- [Known limitations](../docs/known-limitations.md)
- [Release candidate checklist](../docs/release/v0.1.0-rc.md)

## Tạo release artifact

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\build\Publish.ps1 -Version 0.2.0
```

Mã nguồn được phát hành theo giấy phép [GNU GPL v3](../LICENSE). Bản `v0.2.0` được phát hành unsigned; chỉ tải từ GitHub Release chính thức và xác minh SHA-256 trước khi chạy. Không commit file `.pfx` hoặc mật khẩu vào repository nếu bổ sung ký số trong tương lai.

Lệnh tạo self-contained portable ZIP và SHA-256 trong `artifacts/release`. Thêm `-BuildInstaller` khi máy đã cài Inno Setup 6. Gỡ installer không xóa dữ liệu người dùng trong `%LocalAppData%\AuroraDesktopOverlay`.

## Kiến trúc

- `Core`: model và abstraction, không phụ thuộc UI/Windows.
- `Overlay`: vòng đời overlay và WPF overlay window.
- `Media`: kiểm tra media; `Platform.Windows`: Win32 interop.
- `Persistence`, `Infrastructure`: lưu trữ và hạ tầng dùng chung.
- `App`: composition root và Control Center.

Đây là nền móng Sprint 0. Kế hoạch đầy đủ nằm trong `../docs/sprinting`.
