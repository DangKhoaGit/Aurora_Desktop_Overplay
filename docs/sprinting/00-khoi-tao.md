# Giai đoạn 0 — Khởi tạo dự án

## Sprint 0 (1 tuần): Nền móng và giảm rủi ro

Mục tiêu: tạo repository có thể build/test tự động và chứng minh các giả định kỹ thuật rủi ro nhất trước khi phát triển sản phẩm.

### Backlog

- Chốt tên sản phẩm, license, đối tượng phát hành và Windows tối thiểu.
- Tạo solution theo module: `App`, `Core`, `Overlay`, `Media`, `Platform.Windows`, `Persistence`, `Infrastructure` và các project test.
- Thiết lập DI, logging, configuration, nullable reference types, analyzers và format.
- Tạo shell WPF/MVVM tối thiểu và composition root; cấm phụ thuộc ngược từ Core vào WPF/Win32.
- Spike cửa sổ trong suốt/borderless/topmost, `WS_EX_TOOLWINDOW`, `WS_EX_NOACTIVATE`, `WS_EX_TRANSPARENT`.
- Spike hiển thị PNG và GIF; ghi baseline CPU/RAM/GPU, startup time và cách đo.
- Tạo test fixtures cho PNG/JPG/GIF hợp lệ, corrupt, dung lượng lớn và nhiều DPI.
- Khởi tạo Git, `.gitignore`, `README`, `LICENSE`, `CONTRIBUTING`, `SECURITY`, issue/PR templates và `CHANGELOG`.
- Tạo GitHub Actions build/test trên Windows; bật Dependabot và CodeQL nếu repository public.
- Tạo GitHub milestones `v0.1`, `v0.2`, `v0.3`, `v1.0` và labels theo loại/ưu tiên/module.

### Tiêu chí nghiệm thu

- Clone mới có thể restore, build và test bằng lệnh ghi trong README.
- CI chạy tự động trên pull request và nhánh `main`.
- Demo được một PNG trong cửa sổ transparent/topmost mà không xuất hiện ở Alt+Tab.
- Có báo cáo spike nêu rõ API được chọn, giới hạn exclusive fullscreen/taskbar và baseline hiệu năng.
- Architecture tests hoặc quy ước project reference ngăn `Core` phụ thuộc UI/platform.

### Không thuộc sprint

- UI hoàn chỉnh, media library, profile, installer và auto-update.
