# Sprint 6 — Nhật ký kiểm tra

Ngày thực hiện: 25/09/2026

## Đã triển khai

- Chốt product/file metadata và version MVP `0.1.0`; executable là `AuroraDesktopOverlay.exe`.
- Thêm chế độ `--smoke-test` khởi tạo host, monitor và config repository rồi thoát không mở UI/engine.
- Accessibility cơ bản: access key, keyboard tab cycle, automation name/live region, tooltip cho click-through/Edit Mode/Z-order, focus mặc định và panel có scroll.
- Control Center có min-size để tránh mất controls ở cửa sổ nhỏ.
- User guide, troubleshooting, privacy/security, known limitations và RC checklist.
- Script `build/Publish.ps1` tạo self-contained single-file portable ZIP và SHA-256.
- Inno Setup recipe tạo installer per-user, không yêu cầu admin và không xóa AppData khi uninstall.
- GitHub Release workflow: restore/build/test, package portable+installer, smoke test, checksum, upload artifact và tạo release từ tag.
- CI bổ sung publish check để lỗi đóng gói được phát hiện trên pull request.

## Kết quả kiểm tra tự động

- [x] `dotnet restore --locked-mode`.
- [x] `dotnet format --verify-no-changes --no-restore`.
- [x] `dotnet build -c Release --no-restore` — 0 warning, 0 error.
- [x] Toàn bộ unit/integration tests — 24/24 test đạt.
- [x] `build/Publish.ps1 -Version 0.1.0` tạo portable ZIP/checksum; SHA-256 khớp.
- [x] Published executable chạy `--smoke-test` và exit code 0.
- [x] Installer compile bằng Inno Setup 6.7.3; silent install/smoke test/uninstall đều exit code 0.

Installer đã được xác minh trên máy Windows 11, một màn hình 1920×1200. Uninstall xóa ứng dụng và giữ lại thư mục dữ liệu người dùng.

## Ma trận kiểm thử thủ công bắt buộc trước `v0.1.0`

| Nhóm | Ma trận | Trạng thái |
|---|---|---|
| OS | Windows 10 22H2 x64; Windows 11 x64 | Windows 11: installer smoke đạt; Windows 10 chưa chạy |
| DPI | 100%, 125%, 150%, 175%, 200% | Chưa chạy |
| Monitor | 1 màn hình; 2 màn hình cùng/khác DPI; hot-plug | Đã xác nhận máy test 1 màn hình 1920×1200; functional matrix chưa chạy |
| Overlay count | 1, 5, 10, 20 PNG | Chưa chạy |
| GIF count | 1, 5, 10; visible/hidden/opacity 0 | Chưa chạy |
| Lifecycle | sleep/wake; Explorer restart; sign out; shutdown | Chưa chạy |
| Packaging | clean install; upgrade; uninstall; reinstall; portable | Silent install/uninstall và installed smoke đạt; upgrade/reinstall/UI chưa chạy |

## Performance/soak gate

| Kịch bản | Mục tiêu | Kết quả |
|---|---:|---:|
| Idle, không overlay | CPU < 1%; RAM 50–150 MB | Chưa đo |
| 1 PNG tĩnh | CPU gần 0% | Chưa đo |
| 1/5/10 GIF | Ghi CPU/GPU/RAM/FPS | Chưa đo |
| Soak 8 giờ | Không crash/leak tăng liên tục | Chưa chạy |

## Release blockers còn mở

- Đã chọn và thêm GNU GPL-3.0; cần bảo đảm source/tag tương ứng luôn công khai cùng binary phát hành.
- Đã chốt `v0.1.0` phát hành unsigned; release notes phải cảnh báo SmartScreen và cung cấp SHA-256.
- Installer UI, upgrade và reinstall vẫn cần kiểm thử thủ công; silent install/uninstall-preserves-AppData đã đạt.
- Chưa hoàn thành Windows 10/11, DPI, multi-monitor và performance matrix.

## Chính sách go/no-go

- Không tag `v0.1.0` nếu còn P0/P1, test tự động đỏ hoặc chưa chứng minh config/media sống sót qua upgrade/uninstall.
- Có thể tag `v0.1.0-rc.1` để chạy beta sau khi portable/installer và smoke test đạt.
- P2 chỉ được giữ khi có workaround, issue, owner và ghi trong known limitations.
