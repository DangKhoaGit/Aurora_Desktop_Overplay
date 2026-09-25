# Sprint 0 — Nhật ký kiểm tra

Ngày thực hiện: 24/09/2026

## Đã triển khai

- Solution .NET 10 gồm 7 module sản phẩm và 2 project test.
- WPF composition root dùng Generic Host, DI, configuration và logging.
- Spike overlay ảnh tĩnh: transparent, borderless, topmost, không taskbar/Alt+Tab, kéo và click-through (`F2`).
- Model/interface nền tảng, OverlayManager và Win32 interop cô lập.
- Analyzer, central packages, editorconfig, SDK pinning, GitHub Actions và template cộng tác.

## Quyết định kỹ thuật

- Namespace/solution: `Aurora.Desktop.Overlay`; thư mục giữ `Aurora_Desktop_Overplay` theo yêu cầu.
- `Core` dùng `net10.0`, không tham chiếu WPF/Win32; module Windows dùng `net10.0-windows`.
- GIF renderer và benchmark nhiều GIF thuộc Sprint 3.
- Không injection, game-memory read, DirectX hook hoặc DLL injection.

## Cần chủ dự án quyết định

- License (MIT/Apache-2.0/proprietary); chưa tạo `LICENSE` để không tự quyết quyền phân phối.
- GitHub owner, repository name và public/private trước khi tạo remote/push.
- Code-signing certificate.

## Quyết định đã được chốt sau Sprint 0

- Tên sản phẩm: **Aurora Desktop Overlay (ADO)**.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode` — thành công, 9 project có lock file.
- [x] `dotnet format --verify-no-changes --no-restore` — thành công.
- [x] `dotnet build -c Release --no-restore` — thành công, 0 warning, 0 error.
- [x] `dotnet test -c Release --no-build` — 3/3 test thành công.

## Cần kiểm tra thủ công

- [ ] PNG/JPG hiển thị, kéo được, không có taskbar/Alt+Tab.
- [ ] `F2` cho click đi qua overlay; `Esc` đóng overlay.
- [ ] Baseline CPU/RAM/GPU trên Windows 10 và 11.
- [ ] DPI 100%, 150%, 200%.

Các ô được cập nhật theo kết quả thực tế; kiểm thử UI/DPI cần phiên desktop tương tác.
